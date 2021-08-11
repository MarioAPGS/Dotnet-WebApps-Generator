using Core.Interface;
using Core.Models.Interfaces;
using Core.Models.Interfaces.Security.DbItem;
using System;

namespace Core.Models.Security.DbItem
{
    public class Grant : IDbItem
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public string UserId { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }
        public string Method { get; set; }
        public bool Value { get; set; }

        public Grant(string userId, int tableId, string method, bool value)
        {
            UserId = userId;
            TableId = tableId;
            Method = method;
            Value = value;
        }
    }
}
