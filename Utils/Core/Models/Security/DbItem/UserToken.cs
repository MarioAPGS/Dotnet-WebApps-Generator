using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.Interfaces.Security.DbItem
{
    public class UserToken
    {
        [NotMapped]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public long OverdueTime { get; set; }
    }
}
