using Core.Models;
using Core.Models.Interfaces.Security.DbItem;
using Core.Models.Security.DbItem;
using Infrastructure.Repositories.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logic.Managers
{
    public class SecurityManager
    {
        IRepositorySecurity _repositorySecurity;

        public SecurityManager(IRepositorySecurity repositorySecurity)
        {
            _repositorySecurity = repositorySecurity;
        }
        public string GetTokenByRequest(HttpRequest request)
        {
            if (request.Headers.ContainsKey("token"))
                return request.Headers["token"].ToString();
            else
                return null;
        }

        public Response<Log> LogInfo(HttpRequest request, string table, int datachange = 0, string description = null)
        {
            var tool = Functions.GetTool();
            var ipAdress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            return _repositorySecurity.LogInfo(Log.Info(tool, table, request.Method, ipAdress, datachange, description));
        }

        public Response<Log> LogWarn( HttpRequest request, string table, int datachange = 0, string description = null)
        {
            var tool = Functions.GetTool();
            var ipAdress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            return _repositorySecurity.LogWarn(Log.Warn(tool, table, request.Method, ipAdress, datachange, description));
        }

        public Response<Log> LogError( HttpRequest request, string table, string description = null)
        {
            var tool = Functions.GetTool();
            var ipAdress = request.HttpContext.Connection.RemoteIpAddress.ToString();
            return _repositorySecurity.LogError(Log.Error(tool, table, request.Method, ipAdress, description));
        }

        public Response AddOrExisistApplication()
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
            return _repositorySecurity.CreateTool(tool);
        }
    }
}
