using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Infrastructure.AppSettings
{
    public class AppSettings
    {
        public int RegistrationCodeValidity { get; set; }
        public int TokenValidity { get; set; }
        public AuthenticationSettings? AuthenticationSettings { get; set; }

    }
}
