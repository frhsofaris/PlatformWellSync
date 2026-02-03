using Microsoft.EntityFrameworkCore;

namespace PlatformWellSync
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Well> Wells { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=platformwell.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Platform>()
                .HasMany<Well>()
                .WithOne(w => w.Platform)
                .HasForeignKey(w => w.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}