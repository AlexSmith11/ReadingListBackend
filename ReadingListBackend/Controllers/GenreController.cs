using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Database;
using ReadingListBackend.Models;
using ReadingListBackend.Requests;
using ReadingListBackend.Responses;
using ReadingListBackend.Utilities;

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
        public async Task<ActionResult<PaginatedResponse<GenreResponse>>> GetGenres(int page = 1, int pageSize = 10)
        {
            var query = _context.Genres.AsQueryable();
            var totalItems = await query.CountAsync();
            var genres = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GenreResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new PaginatedResponse<GenreResponse>
            {
                Items = genres,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return Ok(response);
        }
        
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
        public async Task<ActionResult<GenreResponse>> Create(GenreCreateRequest genreRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var genre = new Genre { Name = genreRequest.Name };

            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
            
            var genreResponse = _mapper.Map<GenreResponse>(genre);

            return CreatedAtAction(nameof(Get), new {id = genre.Id}, genreResponse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GenreResponse>> UpdateGenre(int id, GenreUpdateRequest genreUpdateRequest)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            if (!string.IsNullOrEmpty(genreUpdateRequest.Name)) 
                genre.Name = genreUpdateRequest.Name;
            
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
            
            var updatedGenre = _mapper.Map<GenreResponse>(genre);

            return Ok(updatedGenre);
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