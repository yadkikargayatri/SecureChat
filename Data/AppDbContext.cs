
using Microsoft.EntityFrameworkCore;
using SecureApp.Model;


namespace SecureChat.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

    }

}   