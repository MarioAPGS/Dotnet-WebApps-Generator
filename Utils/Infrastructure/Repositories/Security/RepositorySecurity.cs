using Core.Models;
using Core.Models.Interfaces.Security.DbItem;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.IO;
using System;
using Microsoft.Extensions.Configuration;
using Core.Models.Security.DbItem;
using Newtonsoft.Json;

namespace Infrastructure.Repositories.Security
{
    public class RepositorySecurity : IRepositorySecurity
    {
        private static readonly log4net.ILog Logging = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly string email = "extremis1999@gmail.com";
        public readonly string password = "Secreto@123";
        private DateTime TimeToOverdueToken;
        private string Token, EpTool, Endpoint, EpValidateToken, EpInfo, EpWarn, EpError, EpCreateTool, EpLogin;
        private bool IsAuthAvailable, IsLogsAvailable;

        public void LoadSettings()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();
            var LogAndAuth = Configuration.GetSection("LogAndAuth");
            EpTool = Configuration.GetSection("Tool").Value;
            Endpoint = LogAndAuth.GetSection("Endpoint").Value;
            EpValidateToken = LogAndAuth.GetSection("Token").Value;
            EpInfo = LogAndAuth.GetSection("Info").Value;
            EpWarn = LogAndAuth.GetSection("Warn").Value;
            EpError = LogAndAuth.GetSection("Error").Value;
            EpCreateTool = LogAndAuth.GetSection("EpCreateTool").Value;
            EpLogin = LogAndAuth.GetSection("EpLogin").Value;
            IsAuthAvailable = Convert.ToBoolean(Configuration.GetSection("Auth").Value);
            IsLogsAvailable = Convert.ToBoolean(Configuration.GetSection("Logs").Value);
            GetToken();
        }

        public Response<Log> LogInfo(Log log)
        {
            try {
                if (EpTool == null || DateTime.Now >= TimeToOverdueToken)
                    LoadSettings();
                if (IsLogsAvailable)
                {    
                    var client = new RestClient();
                    var request = new RestRequest(new Uri(Endpoint + "/" + EpInfo), Method.POST)
                    {
                        RequestFormat = DataFormat.Json
                    };
                    request.AddJsonBody(log);

                    var response = client.Execute<Response<Log>>(request);
                    if (!string.IsNullOrEmpty(response.Content))
                        return response.Data;
                    else
                        throw new Exception("Error reading reponse -> " + response.Content);
                }
                else
                    return new Response<Log>(true, new List<Log>() { log }, "Logs are disabled", HttpStatusCode.Continue);
            }
            catch (Exception ex)
            {
                Logging.Info("LogWarn error");
                Logging.Error(ex.Message);

                return new Response<Log>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<Log> LogWarn(Log log)
        {

            try {
                

                if (EpTool == null || DateTime.Now >= TimeToOverdueToken)
                    LoadSettings();
                if (IsLogsAvailable)
                {
                    var client = new RestClient();
                    var request = new RestRequest(new Uri(Endpoint + "/" + EpWarn), Method.POST)
                    {
                        RequestFormat = DataFormat.Json
                    };
                    request.AddJsonBody(log);

                    var response = client.Execute<Response<Log>>(request);
                    if (!string.IsNullOrEmpty(response.Content))
                        return response.Data;
                    else
                        throw new Exception("Error reading reponse -> " + response.Content);
                }
                else
                    return new Response<Log>(true, new List<Log>() { log }, "Logs are disabled", HttpStatusCode.Continue);
            }
            catch (Exception ex)
            {
                Logging.Info("LogWarn error");
                Logging.Error(ex.Message);

                return new Response<Log>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response<Log> LogError(Log log)
        {
            try
            {
                if (EpTool == null || DateTime.Now >= TimeToOverdueToken)
                    LoadSettings();
                if (IsLogsAvailable)
                {
                    var client = new RestClient();
                    var request = new RestRequest(new Uri(Endpoint + "/" + EpError), Method.POST)
                    {
                        RequestFormat = DataFormat.Json
                    };
                    request.AddJsonBody(log);

                    var response = client.Execute<Response<Log>>(request);
                    if (!string.IsNullOrEmpty(response.Content))
                        return response.Data;
                    else
                        throw new Exception("Error reading reponse -> " + response.Content);
                }
                else
                    return new Response<Log>(true, new List<Log>() { log }, "Logs are disabled", HttpStatusCode.Continue);
            }
            catch (Exception ex)
            {
                Logging.Info("LogError error");
                Logging.Error(ex.Message);

                return new Response<Log>(false, 0, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response ValidateToken(string token, string table, string method)
        {
            try
            {
                if (EpTool == null)
                    LoadSettings();

                if (IsAuthAvailable)
                {
                    var client = new RestClient();
                    var request = new RestRequest(Endpoint + "/" + EpValidateToken, Method.POST);

                    request.AddHeader("token", token);
                    request.RequestFormat = DataFormat.Json;
                    request.AddHeader("tool", EpTool);
                    request.AddHeader("table", CheckTable(table));
                    request.AddHeader("method", method);

                    var response = client.Execute<Response>(request);
                    if (!string.IsNullOrEmpty(response.Content))
                        return response.Data;
                    else
                        throw new Exception("Error reading reponse -> " + response.Content);
                }
                else
                    return new Response(true, "OK", HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                Logging.Info("LogInfo error");
                Logging.Error(ex.Message);

                return new Response(false, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        public Response CreateTool(Tool tool)
        {
            try
            {
                if (EpTool == null || DateTime.Now >= TimeToOverdueToken)
                    LoadSettings();

                if (IsAuthAvailable)
                {
                    var client = new RestClient();
                    var request = new RestRequest(Endpoint + "/" + EpCreateTool, Method.POST);

                    request.AddJsonBody(tool);
                    request.AddHeader("token", Token);

                    var response = client.Execute<Response>(request);
                    if (!string.IsNullOrEmpty(response.Content))
                        return response.Data;
                    else
                        throw new Exception("Error reading reponse -> " + response.Content);
                }
                else
                    return new Response(true, "OK", HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                Logging.Info("LogInfo error");
                Logging.Error(ex.Message);

                return new Response(false, ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        #region Internal Funtions
        private string GetToken()
        {
            try
            {
                var loginResponse = LogIn(email, password);
                if (loginResponse.Success)
                {
                    Token = loginResponse.Data.FirstOrDefault().Value;
                    TimeToOverdueToken = TimeToOverdueToken = DateTime.Now.AddMinutes(loginResponse.Data.FirstOrDefault().OverdueTime - 10);
                }

            }
            catch (Exception ex)
            {
                Logging.Info("LogError error");
                Logging.Error(ex.Message);
            }
            // If token was sent but is user is not loged, Security return forbbiden code
            

            return Token;
        }
        private Response<UserToken> LogIn(string email, string pass)
        {
            try
            {
                if (EpTool == null)
                    LoadSettings();

                var client = new RestClient();
                var request = new RestRequest(Endpoint + "/" + EpLogin, Method.GET);

                request.AddParameter("email", email);
                request.AddParameter("password", pass);
                request.AddHeader("token", "-");
                //request.AddJsonBody(new Log() { Tool = EpTool, Table = table });

                var response = client.Execute(request);
                if (!string.IsNullOrEmpty(response.Content))
                {
                    return JsonConvert.DeserializeObject<Response<UserToken>>(response.Content);
                }
                else
                    throw new Exception("Error reading reponse -> " + response.Content);

            }
            catch (Exception ex)
            {
                Logging.Info("LogInfo error");
                Logging.Error(ex.Message);

                return new Response<UserToken>(false,0 , ex.Message, HttpStatusCode.InternalServerError);
            }
        }
        private string CheckTable(string table)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json");
                IConfiguration Configuration = builder.Build();
                var tables = Configuration.GetSection("Tables").GetChildren().Where(x => x.Value.ToUpper().Trim() == table.ToUpper().Trim());
                if (tables.Any())
                    return tables.First().Value;
                else
                    throw new Exception("Table must be included in the table's section of appsettings.json");
            }
            catch (Exception ex)
            {
                Logging.Error("CheckTable error");
                Logging.Error(ex);
                throw new Exception("Unable verify table '" + table + "'");
            }
        }
        #endregion
    }
}
