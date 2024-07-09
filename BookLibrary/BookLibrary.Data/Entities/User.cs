using BookLibrary.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Data.Entities
{
    public class User : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }

        public bool? IsActive { get; set; }

        public string? DisplayName { get; set; }

        public string? Password { get; set; }

        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public Role Role { get; set; } 

        public string? RegistrationCode { get; set; }
        public DateTime? RegistrationCodeTime { get; set; }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public DateTime? LastLoginTime { get; set; }
    }
}
