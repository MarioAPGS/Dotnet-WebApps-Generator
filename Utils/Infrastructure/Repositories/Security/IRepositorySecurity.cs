﻿using Core.Models;
using Core.Models.Interfaces.Security.DbItem;

namespace Infrastructure.Repositories.Security
{
    public interface IRepositorySecurity
    {
        public Response<Log> LogInfo(Log log);
        public Response<Log> LogWarn(Log log);
        public Response<Log> LogError(Log log);
        public Response ValidateToken(string token);
    }
}
