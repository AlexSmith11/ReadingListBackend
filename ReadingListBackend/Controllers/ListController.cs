using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Database;
using ReadingListBackend.Models;
using ReadingListBackend.Requests;
using ReadingListBackend.Services;

namespace ReadingListBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ListService _listService;

        public ListController(AppDbContext context, ListService listService)
        {
            _context = context;
            _listService = listService ?? throw new ArgumentNullException(nameof(listService));
        }

        // should change this to only send list of names and id's for top level ui
        [HttpGet]
        public async Task<ActionResult<IEnumerable<List>>> Get()
        {
            var lists = await _context.Lists.ToListAsync();
            return lists;
        }

        /// <summary>
        /// Get Single List
        /// TODO: Refactor into service
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        [HttpGet("{listId}")]
        public async Task<ActionResult<List>> Get(int listId)
        {
            var list = await _context.Lists
                .Include(l => l.ListBooks) // include ListBooks for eager loading
                .FirstOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                return NotFound();
            }

            return list;
        }

        /// <summary>
        /// Create a new List
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

        [HttpDelete("RemoveBookFromList")]
        public async Task<IActionResult> RemoveBookFromList([FromBody] RemoveBookFromListDeleteRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _listService.RemoveBookFromList(request.ListId, request.BookId);

            if (result)
            {
                return Ok("Book removed from list successfully.");
            }

            return NotFound("Book is not in the list.");
        }

        private bool ListExists(int listId)
        {
            return _context.Lists.Any(l => l.Id == listId);
        }
    }
}