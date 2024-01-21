using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Database;
using ReadingListBackend.Models;

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

        [HttpPost]
        public async Task<ActionResult<UserListBook>> PostUserListBook(UserListBook userListBook)
        {
            _context.UserListBooks.Add(userListBook);
            await _context.SaveChangesAsync();

            // Return a minimal response, as the primary interaction might happen through ListController
            return Ok();
        }

        [HttpPut("{bookId}/{listId}/{userId}")]
        public async Task<IActionResult> PutUserListBook(int bookId, int listId, int userId, UserListBook userListBook)
        {
            if (bookId != userListBook.BookId || listId != userListBook.ListId || userId != userListBook.UserId)
            {
                return BadRequest();
            }

            _context.Entry(userListBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserListBookExists(bookId, listId, userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Return a minimal response, as the primary interaction might happen through ListController
            return Ok();
        }

        private bool UserListBookExists(int bookId, int listId, int userId)
        {
            return _context.UserListBooks.Any(ulb => ulb.BookId == bookId && ulb.ListId == listId && ulb.UserId == userId);
        }
    }
}