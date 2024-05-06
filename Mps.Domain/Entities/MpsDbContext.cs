using Microsoft.EntityFrameworkCore;
namespace Mps.Domain.Entities
{
    public class MpsDbContext(DbContextOptions<MpsDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.Role).IsRequired();
            modelBuilder.Entity<User>().HasIndex(u => u.IdentityId).IsUnique();
        }
    }
}
