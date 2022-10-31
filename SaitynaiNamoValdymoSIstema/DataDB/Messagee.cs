using System;
using System.Collections.Generic;

namespace SaitynaiNamoValdymoSIstema.DataDB
{
    public partial class Messagee
    {
        public int Id { get; set; }
        public string? TextMessage { get; set; }
        public int PersonId { get; set; }
    }
}
