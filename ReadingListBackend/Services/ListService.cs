using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Database;
using ReadingListBackend.Models;

namespace ReadingListBackend.Services
{
    public class ListService
    {
        private readonly AppDbContext _context;

        public ListService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<bool> CreateList(string name, int userId)
        {
            // Check if the user exists (you may want to perform additional validation)
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            var newList = new List
            {
                Name = name,
                UserId = userId
            };

            // Add the new list to the database
            await _context.Lists.AddAsync(newList);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddBookToList(int listId, int bookId, bool isRead, int position)
        {
            var list = await _context.Lists.FindAsync(listId);
            var book = await _context.Books.FindAsync(bookId);

            // check if List and Book exist
            if (list == null || book == null)
            {
                return false;
            }

            var userListBook = new ListBook
            {
                ListId = listId,
                BookId = bookId,
                IsRead = isRead,
                Position = position
            };

            await _context.ListBooks.AddAsync(userListBook);
            await _context.SaveChangesAsync();

            return true;
        }
        
        public async Task<bool> RemoveBookFromList(int listId, int bookId)
        {
            var listBook = await _context.ListBooks
                .FirstOrDefaultAsync(ulb => ulb.ListId == listId && ulb.BookId == bookId);

            if (listBook == null)
            {
                // Book is not in the list
                return false;
            }

            // Remove the ListBook entry
            _context.ListBooks.Remove(listBook);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}