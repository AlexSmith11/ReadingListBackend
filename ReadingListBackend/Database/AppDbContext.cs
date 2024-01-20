using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Models;

namespace ReadingListBackend.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BookList> BookLists { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User to BookList one-to-many relationship 
            modelBuilder.Entity<BookList>()
                .HasOne(bl => bl.User)
                .WithMany(u => u.BookLists)
                .HasForeignKey(bl => bl.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            // Book and Author many-to-one relationship
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book and BookList many-to-one relationship 
            modelBuilder.Entity<Book>()
                .HasOne(b => b.BookList)
                .WithMany(bl => bl.Books)
                .HasForeignKey(b => b.BookListId)
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