using System.Text;

namespace DevChallenge.Helpers
{
    public static class ResourcesHelper
    {
        public static string GetSqlScript()
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

            return sb.ToString();
        }
    }
}
