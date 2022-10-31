using System;
using System.Collections.Generic;

namespace SaitynaiNamoValdymoSIstema.DataDB
{
    public partial class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public bool IsApproved { get; set; }
        public int FlatId { get; set; }
        public byte[] Password { get; set; } = null!;
        public string? Role { get; set; }
    }
}
