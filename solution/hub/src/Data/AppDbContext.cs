using Microsoft.EntityFrameworkCore;
using Hub.Models;

namespace Hub.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<LinkModel> Links { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.Files)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileModel>()
                .HasMany(f => f.Links)
                .WithOne(l => l.File)
                .HasForeignKey(l => l.FileModelId);

            modelBuilder.Entity<FileModel>()
                .Property(f => f.Status)
                .HasConversion<string>();

            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Username)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}