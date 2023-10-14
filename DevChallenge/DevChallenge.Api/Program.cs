using DevChallenge.Data.Contexts;
using DevChallenge.Helpers;
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

            app.MapGet("/ibges", ([FromServices] DevChallengeDbContext context) =>
            {
                try
                {
                    var ibges = context.Ibges
                                       .AsNoTracking()
                                       .OrderBy(x => x.City)
                                       .ToList();

                    return Results.Ok(ibges);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapGet("/ibges/{id}", ([FromServices] DevChallengeDbContext context, [FromRoute] string id) =>
            {
                try
                {
                    var ibge = context.Ibges
                                      .AsNoTracking()
                                      .FirstOrDefault(x => x.Id == id);

                    if (ibge == null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(ibge);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.MapGet("/ibges/cities/{city}", ([FromServices] DevChallengeDbContext context, [FromRoute] string city) =>
            {
                try
                {
                    var ibges = context.Ibges
                                       .AsNoTracking()
                                       .Where(x => x.City.ToLower() == city.ToLower())
                                       .OrderBy(x => x.State)
                                       .ToList();

                    return Results.Ok(ibges);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });
        }
    }
}