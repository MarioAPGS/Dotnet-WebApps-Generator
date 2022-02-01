using Core.Models;
using Core.Models.Interfaces.Security.DbItem;
using Core.Models.Security.DbItem;
using Infrastructure.Repositories.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logic.Managers
{
    public class SecurityManager
    {
        public IRepositorySecurity RepositorySecurity;

        public SecurityManager(IRepositorySecurity repositorySecurity)
        {
            RepositorySecurity = repositorySecurity;
        }
        public Response ValidateToken(HttpRequest request, string table, string method)
        {
            string token = GetTokenByRequest(request);
            return RepositorySecurity.ValidateToken(token, table, method);
        }
        public Response<Log> LogInfo(HttpRequest request, string table, int datachange = 0, string description = null)
        {
            var tool = Functions.GetTool();
            var ipAdress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            return RepositorySecurity.LogInfo(Log.Info(tool, table, request.Method, ipAdress, datachange, description));
        }
        public Response<Log> LogWarn( HttpRequest request, string table, int datachange = 0, string description = null)
        {
            var tool = Functions.GetTool();
            var ipAdress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            return RepositorySecurity.LogWarn(Log.Warn(tool, table, request.Method, ipAdress, datachange, description));
        }
        public Response<Log> LogError( HttpRequest request, string table, string description = null)
        {
            var tool = Functions.GetTool();
            var ipAdress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            return RepositorySecurity.LogError(Log.Error(tool, table, request.Method, ipAdress, description));
        }
        public Response<UserToken> LogIn(string email, string password)
        {
            return RepositorySecurity.LogIn(email, password);
        }
        public Response ValidateLogin(HttpRequest request)
        {
            string token = null;
            if (request.Headers.ContainsKey("token"))
                token = request.Headers["token"].ToString();
            return RepositorySecurity.ValidateLogin(token);
        }

        public string GetTokenByRequest(HttpRequest request)
        {
            string token = "";
            if (request.Headers.ContainsKey("token"))
                token = request.Headers["token"].ToString();

            return token;
        }

        public string AddOrExisistApplication()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();
            var toolName = Configuration.GetSection("Tool").Value;
            var tables = Configuration.GetSection("Tables").GetChildren().ToList();

            Tool tool = new(toolName);
            var tablesList = new List<Table>();
            foreach (var table in tables)
                tablesList.Add(new(table.Value));
            tool.Tables = tablesList;
            return JsonConvert.SerializeObject(tool);
        }
    }
}
