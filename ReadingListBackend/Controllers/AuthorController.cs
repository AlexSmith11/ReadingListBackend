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

namespace ReadingListBackend.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AuthorController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorResponse>>> Get()
        {
            var authors = await _context.Authors
                .ProjectTo<AuthorResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorResponse>> Get(int id)
        {
            var author  = await _context.Authors
                .Where(a => a.Id == id)
                .ProjectTo<AuthorResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            
            if (author == null) return NotFound();

            return author;
        }

        [HttpPost]
        public async Task<ActionResult<AuthorResponse>> Create(AuthorCreateRequest authorRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var author = new Author
            {
                Name = authorRequest.Name,
                Age = authorRequest.Age,
                Country = authorRequest.Country
            };

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
            
            var authorResponse = _mapper.Map<AuthorResponse>(author);

            return CreatedAtAction(nameof(Get), new { id = author.Id }, authorResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AuthorUpdateRequest authorUpdateRequest)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();
            if (id != author.Id) return BadRequest();
            
            // update values if they have a new value given
            if (!string.IsNullOrEmpty(authorUpdateRequest.Name)) author.Name = authorUpdateRequest.Name;
            if (authorUpdateRequest.Age.HasValue) author.Age = authorUpdateRequest.Age.Value;
            if (!string.IsNullOrEmpty(authorUpdateRequest.Country)) author.Country = authorUpdateRequest.Country;
            
            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}