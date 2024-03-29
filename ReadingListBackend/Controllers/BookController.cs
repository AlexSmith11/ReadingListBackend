﻿using System.Collections.Generic;
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
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BookController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<BookResponse>>> GetBooks(int page = 1, int pageSize = 10)
        {
            var query = _context.Books.AsQueryable();
            var totalItems = await query.CountAsync();
            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<BookResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new PaginatedResponse<BookResponse>
            {
                Items = books,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponse>> Get(int id)
        {
            var book  = await _context.Books
                .Where(b => b.Id == id)
                .ProjectTo<BookResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            
            if (book == null) return NotFound();

            return book;
        }
        
        [HttpPost]
        public async Task<ActionResult<BookResponse>> Create(BookCreateRequest bookRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            // make sure the author and genre already exist within the db
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == bookRequest.AuthorId);
            var genreExists = await _context.Genres.AnyAsync(g => g.Id == bookRequest.GenreId);
            if (!authorExists || !genreExists) return BadRequest("Invalid AuthorId or GenreId");

            var book = new Book
            {
                Title = bookRequest.Title,
                PageCount = bookRequest.PageCount,
                AuthorId = bookRequest.AuthorId,
                GenreId = bookRequest.GenreId,
            };
            
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            
            var bookResponse = _mapper.Map<AuthorResponse>(book);

            return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, bookResponse);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<BookResponse>> Update(int id, BookUpdateRequest bookUpdateRequest)
        {
            var book = await _context.Books.FindAsync(id);
            
            if (book == null) return NotFound();
            if (id != book.Id) return BadRequest();
            
            // update title
            if (!string.IsNullOrEmpty(bookUpdateRequest.Title)) book.Title = bookUpdateRequest.Title;

            // update author
            if (bookUpdateRequest.AuthorId.HasValue)
            {
                var authorExists = await _context.Authors.AnyAsync(a => a.Id == bookUpdateRequest.AuthorId);
                if (!authorExists) return BadRequest("Invalid AuthorId");

                book.AuthorId = bookUpdateRequest.AuthorId.Value;
            }
            
            // update genre
            if (bookUpdateRequest.GenreId.HasValue)
            {
                var genreExists = await _context.Genres.AnyAsync(g => g.Id == bookUpdateRequest.GenreId);
                if (!genreExists) return BadRequest("Invalid GenreId");

                book.GenreId = bookUpdateRequest.GenreId.Value;
            }
            
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id)) return NotFound();
                else throw;
            }
            
            var updatedBook = _mapper.Map<AuthorResponse>(book);

            return Ok(updatedBook);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}