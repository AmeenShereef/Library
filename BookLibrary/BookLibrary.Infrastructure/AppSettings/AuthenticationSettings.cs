using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Infrastructure.AppSettings
{    
    public class AuthenticationSettings
    {
        public JwtBearerSettings JwtBearer { get; set; } = null!;
    }    
}
