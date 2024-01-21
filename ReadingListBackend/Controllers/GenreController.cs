using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Database;
using ReadingListBackend.Models;
using ReadingListBackend.Requests;

namespace ReadingListBackend.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenreController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GenreController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> Get()
        {
            return await _context.Genres.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> Get(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();
            return genre;
        }

        [HttpPost]
        public async Task<ActionResult<Genre>> Create(GenreCreateRequest genreRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var genre = new Genre { Name = genreRequest.Name };

            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new {id = genre.Id}, genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, GenreUpdateRequest genreUpdateRequest)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            if (!string.IsNullOrEmpty(genreUpdateRequest.Name)) genre.Name = genreUpdateRequest.Name;
            
            _context.Entry(genre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null) return NotFound();

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}