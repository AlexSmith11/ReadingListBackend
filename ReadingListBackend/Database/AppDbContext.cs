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
        public DbSet<UserListBook> UserListBooks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User to List relationship
            modelBuilder.Entity<List>()
                .HasOne(l => l.User)
                .WithMany(u => u.Lists)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Book to Author relationship
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserListBook relationships
            modelBuilder.Entity<UserListBook>()
                .HasKey(ulb => new { ulb.BookId, ulb.ListId, ulb.UserId });

            modelBuilder.Entity<UserListBook>()
                .HasOne(ulb => ulb.User)
                .WithMany()
                .HasForeignKey(ulb => ulb.UserId);

            modelBuilder.Entity<UserListBook>()
                .HasOne(ulb => ulb.List)
                .WithMany(list => list.UserListBooks)
                .HasForeignKey(ulb => ulb.ListId);

            modelBuilder.Entity<UserListBook>()
                .HasOne(ulb => ulb.Book)
                .WithMany(book => book.UserListBooks)
                .HasForeignKey(ulb => ulb.BookId);

            // Book to Genre relationship
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Other entity configurations...
        }


    }
}