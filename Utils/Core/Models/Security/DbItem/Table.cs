using Core.Interface;
using Core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Security.DbItem
{
    public class Table : IDbItem
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }

        public string Name { get; set; }
        public int ToolId { get; set; }
        public Tool Tool { get; set; }
        public IEnumerable<Grant> Grants { get; set; }

        public Table(string name)
        {
            Name = name;
        }

        public Table(string name, int toolId) : this(name)
        {
            ToolId = toolId;
        }
    }
}
