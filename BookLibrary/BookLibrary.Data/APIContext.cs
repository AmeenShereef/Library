using BookLibrary.Data.Entities;
using BookLibrary.Data.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookLibrary.Data
{
    public class APIContext : DbContext
    {
        public const string Name = "BookLibrary";
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; } = default!;
        public virtual DbSet<APILog> APILogs { get; set; } = default!;
        public virtual DbSet<User> Users { get; set; } = default!;
        public virtual DbSet<Role> Roles { get; set; } = default!;

        protected readonly IConfiguration? Configuration;

        public APIContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.LoadConfigurationsFromAssembly();

            // Seed roles data
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Admin" },
                new Role { RoleId = 2, Name = "User" }           
            );            
        }
    }
}
