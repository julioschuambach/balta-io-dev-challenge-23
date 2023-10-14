using DevChallenge.Api.ViewModels;
using DevChallenge.Data.Contexts;
using DevChallenge.Helpers;
using DevChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DevChallengeDbContext>();
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
        }
    }
}