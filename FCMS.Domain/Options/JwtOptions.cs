using System;
using System.Collections.Generic;
using System.Text;

namespace FCMS.Domain.Options
{
    public class JwtOptions
    {
        public String SecretKey { get; set; }
        public Int32 ExpiryMinutes { get; set; }
        public String Issuer { get; set; }
        public String Audience { get; set; }
    }
}
