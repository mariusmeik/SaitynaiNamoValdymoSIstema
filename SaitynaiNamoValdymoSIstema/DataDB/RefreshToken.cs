using System;
using System.Collections.Generic;

namespace SaitynaiNamoValdymoSIstema.DataDB
{
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int PersonId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        
    }
}
