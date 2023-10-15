using DevChallenge.Api;
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
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
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
            });

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
            });

            app.MapGet("/locations", ([FromServices] DevChallengeDbContext context) =>
            {
                try
                {
                    var locations = context.Locations
                                       .AsNoTracking()
                                       .OrderBy(x => x.City)
                                       .ToList();

                    return Results.Ok(locations);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapGet("/locations/{id}", ([FromServices] DevChallengeDbContext context, [FromRoute] string id) =>
            {
                try
                {
                    var location = context.Locations
                                      .AsNoTracking()
                                      .FirstOrDefault(x => x.Id == id);

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
            });

            app.MapGet("/locations/cities/{city}", ([FromServices] DevChallengeDbContext context, [FromRoute] string city) =>
            {
                try
                {
                    var locations = context.Locations
                                       .AsNoTracking()
                                       .Where(x => x.City.ToLower() == city.ToLower())
                                       .OrderBy(x => x.State)
                                       .ToList();

                    return Results.Ok(locations);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapGet("/locations/states/{state}", ([FromServices] DevChallengeDbContext context, [FromRoute] string state) =>
            {
                try
                {
                    var locations = context.Locations
                                      .AsNoTracking()
                                      .Where(x => x.State.ToLower() == state.ToLower())
                                      .OrderBy(x => x.City)
                                      .ToList();

                    return Results.Ok(locations);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapPost("/locations", ([FromServices] DevChallengeDbContext context, [FromBody] Location location) =>
            {
                try
                {
                    context.Locations.Add(location);
                    context.SaveChanges();

                    return Results.Created($"/locations/{location.Id}", location);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapPut("/locations/{id}", ([FromServices] DevChallengeDbContext context, [FromRoute] string id, [FromBody] LocationViewModel viewModel) =>
            {
                try
                {
                    var location = context.Locations.FirstOrDefault(x => x.Id == id);

                    if (location == null)
                    {
                        return Results.NotFound();
                    }

                    location.Update(viewModel);
                    context.Locations.Update(location);
                    context.SaveChanges();

                    return Results.Ok(location);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapDelete("/locations/{id}", ([FromServices] DevChallengeDbContext context, [FromRoute] string id) =>
            {
                try
                {
                    var location = context.Locations.FirstOrDefault(x => x.Id == id);

                    if (location == null)
                    {
                        return Results.NotFound();
                    }

                    context.Locations.Remove(location);
                    context.SaveChanges();

                    return Results.Ok(location);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapPost("/signup", ([FromServices] DevChallengeDbContext context, [FromBody] SignUpViewModel viewModel) =>
            {
                if (viewModel.Password != viewModel.ConfirmPassword)
                {
                    return Results.BadRequest("The passwords don't match!");
                }

                try
                {
                    User user = new(viewModel);

                    context.Users.Add(user);
                    context.SaveChanges();

                    return Results.Ok(user);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapPost("/signin", ([FromServices] DevChallengeDbContext context, [FromServices] TokenService tokenService, [FromBody] SignInViewModel viewModel) =>
            {
                try
                {
                    var user = context.Users.FirstOrDefault(x => x.Username == viewModel.Username);

                    if (user == null || user.Password != viewModel.Password)
                    {
                        return Results.BadRequest("Username or password incorrect.");
                    }

                    var token = tokenService.GenerateToken(user);
                    return Results.Ok(token);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapGet("/users", ([FromServices] DevChallengeDbContext context) =>
            {
                try
                {
                    var users = context.Users
                                       .AsNoTracking()
                                       .OrderBy(x => x.Username)
                                       .ToList();

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