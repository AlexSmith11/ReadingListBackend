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
        public DbSet<ListBook> ListBooks { get; set; }

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
            
            // Book to Genre relationship
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // ListBook relationships
            modelBuilder.Entity<ListBook>()
                .HasKey(ulb => new { ulb.BookId, ulb.ListId });

            //      ListBook to List
            modelBuilder.Entity<ListBook>()
                .HasOne(ulb => ulb.List)
                .WithMany(list => list.ListBooks)
                .HasForeignKey(ulb => ulb.ListId)
                .IsRequired();
            
            //      ListBook to Book
            modelBuilder.Entity<ListBook>()
                .HasOne(ulb => ulb.Book)
                .WithMany(book => book.ListBooks)
                .HasForeignKey(ulb => ulb.BookId)
                .IsRequired();
        }
    }
}