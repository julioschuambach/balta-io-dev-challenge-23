using DevChallenge.Api;
using DevChallenge.Api.Data.Interfaces.Repositories;
using DevChallenge.Api.Data.Repositories;
using DevChallenge.Api.Models;
using DevChallenge.Api.Services;
using DevChallenge.Api.ViewModels;
using DevChallenge.Data.Contexts;
using DevChallenge.Helpers;
using DevChallenge.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DevChallenge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureAuthentication(builder);
            ConfigureAuthorization(builder);
            ConfigureServices(builder);
            var app = builder.Build();

            MapEndpoints(app);
            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        private static void ConfigureAuthorization(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(x =>
            {
                x.AddPolicy("Admin", policy => policy.RequireRole("admin"));
                x.AddPolicy("User", policy => policy.RequireRole("user"));
            });
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DevChallengeDbContext>();
            builder.Services.AddTransient<TokenService>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<ILocationRepository, LocationRepository>();
        }

        private static void MapEndpoints(WebApplication app)
        {
            app.MapGet("/sql-script", () =>
            {
                var script = ResourcesHelper.GetSqlScript();

                try
                {
                    return Results.Ok(script);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization("admin");

            app.MapGet("/create-database", ([FromServices] DevChallengeDbContext context) =>
            {
                var script = ResourcesHelper.GetSqlScript();

                try
                {
                    context.Database.ExecuteSqlRaw(script);
                    return Results.Ok("Database created successfully!");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).AllowAnonymous();

            app.MapGet("/locations", ([FromServices] ILocationRepository repository) =>
            {
                try
                {
                    var locations = repository.GetAllLocations();

                    return Results.Ok(locations);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization();

            app.MapGet("/locations/{id}", ([FromServices] ILocationRepository repository, [FromRoute] string id) =>
            {
                try
                {
                    var location = repository.GetLocationById(id);

                    if (location == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(location);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization();

            app.MapGet("/locations/cities/{city}", ([FromServices] ILocationRepository repository, [FromRoute] string city) =>
            {
                try
                {
                    var locations = repository.GetLocationsByCity(city);

                    return Results.Ok(locations);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization();

            app.MapGet("/locations/states/{state}", ([FromServices] ILocationRepository repository, [FromRoute] string state) =>
            {
                try
                {
                    var locations = repository.GetLocationsByState(state);

                    return Results.Ok(locations);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization();

            app.MapPost("/locations", ([FromServices] ILocationRepository repository, [FromBody] Location location) =>
            {
                try
                {
                    repository.CreateLocation(location);

                    return Results.Created($"/locations/{location.Id}", location);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization();

            app.MapPut("/locations/{id}", ([FromServices] ILocationRepository repository, [FromRoute] string id, [FromBody] LocationViewModel viewModel) =>
            {
                try
                {
                    var location = repository.UpdateLocation(id, viewModel);

                    if (location == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(location);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization();

            app.MapDelete("/locations/{id}", ([FromServices] ILocationRepository repository, [FromRoute] string id) =>
            {
                try
                {
                    var location = repository.DeleteLocation(id);

                    if (location == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(location);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization();

            app.MapPost("/signup", ([FromServices] IUserRepository repository, [FromBody] SignUpViewModel viewModel) =>
            {
                if (viewModel.Password != viewModel.ConfirmPassword)
                {
                    return Results.BadRequest("The passwords don't match!");
                }

                try
                {
                    User user = new(viewModel);
                    repository.CreateUser(user);

                    return Results.Ok(user);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapPost("/signin", ([FromServices] IUserRepository repository, [FromServices] TokenService tokenService, [FromBody] SignInViewModel viewModel) =>
            {
                try
                {
                    var user = repository.GetUserByEmail(viewModel.Email);

                    if (user == null || user.Password != viewModel.Password)
                    {
                        return Results.BadRequest("Email or password incorrect.");
                    }

                    var token = tokenService.GenerateToken(user);
                    return Results.Ok(token);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapGet("/users", ([FromServices] IUserRepository repository) =>
            {
                try
                {
                    var users = repository.GetAllUsers();

                    return Results.Ok(users);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization("admin");
        }
    }
}