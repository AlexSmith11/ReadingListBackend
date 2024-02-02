using System;
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
using ReadingListBackend.Services;

namespace ReadingListBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ListService _listService;
        private readonly IMapper _mapper;

        public ListController(AppDbContext context, ListService listService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _listService = listService ?? throw new ArgumentNullException(nameof(listService));
        }

        /// <summary>
        /// Returns only a top level view of the lists - this does not return a tree structure including books, etc
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListSummaryResponse>>> Get()
        {
            var lists = await _context.Lists
                .Select(list => new ListSummaryResponse
                {
                    Id = list.Id,
                    Name = list.Name
                })
                .ToListAsync();

            return Ok(lists);
        }

        /// <summary>
        /// Get Single List
        /// TODO: Refactor into service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ListResponse>> Get(int id)
        {
            var list = await _context.Lists
                .Include(l => l.ListBooks)
                .ThenInclude(lb => lb.Book)
                .ThenInclude(b => b.Author)
                .Include(l => l.ListBooks)
                .ThenInclude(lb => lb.Book)
                .ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (list == null) return NotFound();

            var listResponse = _mapper.Map<ListResponse>(list);

            return listResponse;
        }

        /// <summary>
        /// Create a new List
        /// TODO: Refactor to include the ListResponse class instead 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<List>> Post([FromBody] ListCreateRequest list)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _listService.CreateList(list.Name, list.UserId);

            if (result) return Ok(result);

            return BadRequest("Failed to create the list.");
        }

        /// <summary>
        /// Edit a given list
        /// TODO: Refactor into service
        /// TODO: Refactor to include the ListResponse class instead 
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPut("{listId}")]
        public async Task<IActionResult> Put(int listId, List list)
        {
            if (listId != list.Id)
            {
                return BadRequest();
            }

            _context.Entry(list).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListExists(listId)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a List
        /// TODO: Refactor into service
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        [HttpDelete("{listId}")]
        public async Task<IActionResult> Delete(int listId)
        {
            var list = await _context.Lists.FindAsync(listId);
            if (list == null)
            {
                return NotFound();
            }

            _context.Lists.Remove(list);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // book to list methods

        /// <summary>
        /// Add a Book to a List using ListService
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AddBookToList")]
        public async Task<IActionResult> AddBookToList([FromBody] AddBookToListCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result =
                await _listService.AddBookToList(request.ListId, request.BookId, request.IsRead, request.Position);
            if (result) return Ok("Book added to list successfully.");

            return BadRequest("Failed to add the book to the list.");
        }

        [HttpDelete("RemoveBookFromList")]
        public async Task<IActionResult> RemoveBookFromList([FromBody] RemoveBookFromListDeleteRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _listService.RemoveBookFromList(request.ListId, request.BookId);

            if (result) return Ok("Book removed from list successfully.");

            return NotFound("Book is not in the list.");
        }

        [HttpPost("UpdateBookPosition")]
        public async Task<IActionResult> UpdateBookPosition([FromBody] UpdateBookPositionOnListRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _listService.UpdateBookPosition(request.ListId, request.BookId, request.NewPosition);

            if (result) return Ok("Book position updated successfully.");

            return NotFound("ListBook not found or update failed.");
        }

        // helpers

        private bool ListExists(int listId)
        {
            return _context.Lists.Any(l => l.Id == listId);
        }
    }
}