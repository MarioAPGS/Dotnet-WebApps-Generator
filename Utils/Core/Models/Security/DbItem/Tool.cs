using Core.Interface;
using Core.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace Core.Models.Security.DbItem
{
    public class Tool : IDbItem
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public string Name { get; set; }
        public IEnumerable<Table> Tables {get; set; }

        public Tool (string name)
        {
            Name = name;
        }

        public Tool(string name, IEnumerable<Table> tables) : this(name)
        {
            Tables = tables;
        }
    }
}
