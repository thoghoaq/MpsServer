using Microsoft.EntityFrameworkCore;
namespace Mps.Domain.Entities
{
    public class MpsDbContext(DbContextOptions<MpsDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
        }
    }
}
