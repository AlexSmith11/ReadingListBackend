using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Database;
using ReadingListBackend.Models;
using ReadingListBackend.Requests;
using ReadingListBackend.Responses;

namespace ReadingListBackend.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenreController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GenreController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> Get()
        {
            return await _context.Genres.ToListAsync();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GenreResponse>> Get(int id)
        {
            var genreResponse  = await _context.Genres
                .Where(g => g.Id == id)
                .ProjectTo<GenreResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (genreResponse  == null) return NotFound();

            return Ok(genreResponse);
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