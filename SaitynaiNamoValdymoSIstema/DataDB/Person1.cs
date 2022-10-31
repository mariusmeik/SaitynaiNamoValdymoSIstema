using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaitynaiNamoValdymoSIstema.DataDB
{
    public partial class Person1
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public bool IsApproved { get; set; }=false;
        public int FlatId { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
