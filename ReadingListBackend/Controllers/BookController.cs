using Microsoft.AspNetCore.Mvc;
using ReadingListBackend.Database;

namespace ReadingListBackend.Controllers
{
    [ApiController]
    [Route("book")]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet(Name = "GetConfirmation")]
        public string Get()
        {
            return "Hello";
        }
    }
}