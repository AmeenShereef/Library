using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class UserAdd
    {
        public string DisplayName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string RegistrationCode { get; set; } = default!;
    }
}
