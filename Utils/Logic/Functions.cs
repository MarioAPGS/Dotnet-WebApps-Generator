using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;

namespace Core.Models
{
    public static class Functions
    {
        private static readonly log4net.ILog Logging = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static int? ToNullableInt(string integer)
        {
            if (int.TryParse(integer, out int i)) return i;
            return null;
        }

        public static string RequestToString(HttpRequest request)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[" + request.Method + "] ");

            if (request.IsHttps)
                sb.Append("https://");
            else
                sb.Append("http://");

            sb.Append(request.Host + request.Path);

            return sb.ToString();
        }

        public static string GetTool()
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json");
                IConfiguration Configuration = builder.Build();
                var LogAndAuth = Configuration.GetSection("LogAndAuth");
                return Configuration.GetSection("Tool").Value;
            }
            catch (Exception ex)
            {
                Logging.Error("GetTool error");
                Logging.Error(ex);

                return null;
            }
        }

    }
}
