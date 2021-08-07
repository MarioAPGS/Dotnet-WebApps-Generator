using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Interface
{
    public interface IDbItem
    {
        [Key]
        int Id { get; set; }
        DateTime Created { get; set; }
        DateTime Modified {get; set; }
    }
}
