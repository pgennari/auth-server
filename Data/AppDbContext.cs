// In a new file, e.g., Data/AppDbContext.cs
using auth_server.Models;
using Microsoft.EntityFrameworkCore;

namespace auth_server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<NotificationRecord> NotificationRecords { get; set; }
        public DbSet<Otp> Otps { get; set; } 
    }
}
