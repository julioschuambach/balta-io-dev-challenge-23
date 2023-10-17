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
using Microsoft.OpenApi.Models;
using System.Text;

namespace DevChallenge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            ConfigureServices(builder);
            var app = builder.Build();

            MapEndpoints(app);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DevChallengeDbContext>();
            builder.Services.AddTransient<TokenService>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<ILocationRepository, LocationRepository>();

            ConfigureAuthentication(builder);
            ConfigureAuthorization(builder);
            ConfigureSwagger(builder);
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

        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "balta.io Dev Challenge 23",
                    Contact = new OpenApiContact
                    {
                        Name = "Júlio Schuambach",
                        Url = new Uri("https://github.com/julioschuambach"),
                        Email = "julioschuambach.dev@gmail.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/license/mit/")
                    }
                });
            });
        }

        private static void MapEndpoints(WebApplication app)
        {
            app.MapGet("/v1/sql-script", () =>
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
            }).RequireAuthorization("admin").Produces<string>();

            app.MapGet("/v1/create-database", async ([FromServices] DevChallengeDbContext context) =>
            {
                var script = ResourcesHelper.GetSqlScript();

                try
                {
                    await context.Database.ExecuteSqlRawAsync(script);
                    return Results.Ok("Database created successfully!");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).Produces<string>();

            app.MapGet("/v1/locations", async ([FromServices] ILocationRepository repository) =>
            {
                try
                {
                    var locations = await repository.GetAllLocations();

                    return Results.Ok(locations);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).Produces<IEnumerable<Location>>().RequireAuthorization();

            app.MapGet("/v1/locations/{id}", async ([FromServices] ILocationRepository repository, [FromRoute] string id) =>
            {
                try
                {
                    var location = await repository.GetLocationById(id);

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
            }).Produces<Location>().RequireAuthorization();

            app.MapGet("/v1/locations/cities/{city}", async ([FromServices] ILocationRepository repository, [FromRoute] string city) =>
            {
                try
                {
                    var locations = await repository.GetLocationsByCity(city);

                    return Results.Ok(locations);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).Produces<IEnumerable<Location>>().RequireAuthorization();

            app.MapGet("/v1/locations/states/{state}", async ([FromServices] ILocationRepository repository, [FromRoute] string state) =>
            {
                try
                {
                    var locations = await repository.GetLocationsByState(state);

                    return Results.Ok(locations);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).Produces<IEnumerable<Location>>().RequireAuthorization();

            app.MapPost("/v1/locations", async ([FromServices] ILocationRepository repository, [FromBody] Location location) =>
            {
                try
                {
                    await repository.CreateLocation(location);

                    return Results.Created($"/locations/{location.Id}", location);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).Produces<Location>().RequireAuthorization();

            app.MapPut("/v1/locations/{id}", async ([FromServices] ILocationRepository repository, [FromRoute] string id, [FromBody] LocationViewModel viewModel) =>
            {
                try
                {
                    var location = await repository.UpdateLocation(id, viewModel);

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
            }).Produces<Location>().RequireAuthorization();

            app.MapDelete("/v1/locations/{id}", async ([FromServices] ILocationRepository repository, [FromRoute] string id) =>
            {
                try
                {
                    var location = await repository.DeleteLocation(id);

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
            }).Produces<Location>().RequireAuthorization();

            app.MapPost("/v1/signup", async ([FromServices] IUserRepository repository, [FromBody] SignUpViewModel viewModel) =>
            {
                if (viewModel.Password != viewModel.ConfirmPassword)
                {
                    return Results.BadRequest("The passwords don't match!");
                }

                try
                {
                    User user = new(viewModel);
                    await repository.CreateUser(user);

                    return Results.Ok(user);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).Produces<User>();

            app.MapPost("/v1/signin", async ([FromServices] IUserRepository repository, [FromServices] TokenService tokenService, [FromBody] SignInViewModel viewModel) =>
            {
                try
                {
                    var user = await repository.GetUserByEmail(viewModel.Email);

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
            }).Produces<string>();

            app.MapGet("/v1/users", async ([FromServices] IUserRepository repository) =>
            {
                try
                {
                    var users = await repository.GetAllUsers();

                    return Results.Ok(users);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).Produces<IEnumerable<User>>().RequireAuthorization("admin");
        }
    }
}