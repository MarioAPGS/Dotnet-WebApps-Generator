using Core.Models;
using Core.Models.Interfaces.Security.DbItem;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Infrastructure.Repositories.Security
{
    public class RepositorySecurity : IRepositorySecurity
    {
        private static readonly log4net.ILog Logging = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly string TokenKey = "token";
        public string EpTool { get; set; }
        public string Endpoint { get; set; }
        public string EpValidateToken { get; set; }
        public string EpInfo { get; set; }
        public string EpWarn { get; set; }
        public string EpError { get; set; }
        private bool IsAuthAvailable { get; set; }
        private bool IsLogsAvailable { get; set; }
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
            IsAuthAvailable = Convert.ToBoolean(Configuration.GetSection("Auth").Value);
            IsLogsAvailable = Convert.ToBoolean(Configuration.GetSection("Logs").Value);
        }       

        public Response<Log> LogInfo(Log log)
        {
            try {
                if (EpTool == null)
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
                

                if (EpTool == null)
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
                if (EpTool == null)
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

        public Response ValidateToken(string token)
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
                    //request.AddJsonBody(new Log() { Tool = EpTool, Table = table });

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
    }
}