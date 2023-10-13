using System.Text;

namespace DevChallenge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            MapEndpoints(app);

            app.Run();
        }

        private static void MapEndpoints(WebApplication app)
        {
            app.MapGet("/read-sql-scripts", () =>
            {
                List<string> paths = new()
                {
                    "..\\..\\Resources\\Scripts\\Schema.sql",
                    "..\\..\\Resources\\Scripts\\Data.sql",
                    "..\\..\\Resources\\Scripts\\2021-03-14-06-31_corrige_sigla_estado_roraima.sql"
                };

                StringBuilder sb = new();
                foreach (var path in paths)
                {
                    sb.Append(File.ReadAllText(path));
                }

                try
                {
                    return Results.Ok(sb.ToString());
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });
        }
    }
}