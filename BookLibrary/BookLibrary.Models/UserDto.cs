using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class UserDto
    {    
        public int UserId { get; set; }    
        public string Email { get; set; } = default!;
        public bool? IsActive { get; set; }
        public string? DisplayName { get; set; }       
        public string? Role { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? LastLoginTime { get; set; }
    }
}
