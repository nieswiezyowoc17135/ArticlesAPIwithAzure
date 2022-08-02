using ArticlesAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArticlesAPI.Data
{
    public class ArticlesContext : DbContext
    {
        public ArticlesContext (DbContextOptions options) : base(options)
        {

        }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .Property(x => x.Title)
                .IsRequired();

            modelBuilder.Entity<Article>()
                .Property(x => x.Title)
                .HasMaxLength(30)
                .IsRequired();

            modelBuilder.Entity<Article>()
                .Property(x => x.Description)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Article>()
                .Property(x => x.ImageURL)
                .IsRequired();
        }
    }
}
