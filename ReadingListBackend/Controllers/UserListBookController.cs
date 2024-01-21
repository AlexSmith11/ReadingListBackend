using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadingListBackend.Database;

namespace ReadingListBackend.Controllers
{
    [ApiController]
    [Route("api/userlistbooks")]
    public class UserListBookController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserListBookController(AppDbContext context)
        {
            _context = context;
        }

        // ... (Existing methods)

        // Mark a book as read for a user on a specific list
        [HttpPut("mark-read/{userId}/{listId}/{bookId}")]
        public async Task<IActionResult> MarkBookAsRead(int userId, int listId, int bookId)
        {
            var userListBook = await _context.UserListBooks
                .Where(ulb => ulb.BookId == bookId && ulb.ListId == listId && ulb.UserId == userId)
                .FirstOrDefaultAsync();

            if (userListBook == null)
            {
                return NotFound();
            }

            userListBook.IsRead = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Update the order of a book on a list for a user
        [HttpPut("update-order/{userId}/{listId}/{bookId}/{newOrder}")]
        public async Task<IActionResult> UpdateBookOrder(int userId, int listId, int bookId, int newOrder)
        {
            var userListBook = await _context.UserListBooks
                .Where(ulb => ulb.BookId == bookId && ulb.ListId == listId && ulb.UserId == userId)
                .FirstOrDefaultAsync();

            if (userListBook == null)
            {
                return NotFound();
            }

            userListBook.Order = newOrder;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}