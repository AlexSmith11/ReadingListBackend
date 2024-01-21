using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Models;

namespace ReadingListBackend.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User to List one-to-many relationship 
            modelBuilder.Entity<List>()
                .HasOne(l => l.User)
                .WithMany(u => u.Lists)
                .HasForeignKey(bl => bl.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            // Book and Author many-to-one relationship
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book and List many-to-one relationship 
            modelBuilder.Entity<Book>()
                .HasOne(b => b.List)
                .WithMany(l => l.Books)
                .HasForeignKey(b => b.ListId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book and Genre many-to-one relationship 
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}