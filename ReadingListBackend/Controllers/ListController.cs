using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReadingListBackend.Exceptions;
using ReadingListBackend.Interfaces;
using ReadingListBackend.Models;
using ReadingListBackend.Requests;
using ReadingListBackend.Responses;
using ReadingListBackend.Utilities;

namespace ReadingListBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListController : ControllerBase
    {
        private readonly IListService _listService;

        public ListController(IListService listService)
        {
            _listService = listService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ListSummaryResponse>>> GetLists(int page = 1, int pageSize = 10)
        {
            try
            {
                var paginatedResponse = await _listService.GetAllListsAsync(page, pageSize);
                return Ok(paginatedResponse);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ListResponse>> Get(int id)
        {
            try
            {
                var listResponse = await _listService.GetListByIdAsync(id);
                return Ok(listResponse);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<List>> Post([FromBody] ListCreateRequest list)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var listResponse = await _listService.CreateListAsync(list);
                return CreatedAtAction(nameof(Get), new { id = listResponse.Id }, listResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Message = "Internal Server Error" });
            }
        }
        
        /// <summary>
        /// TODO: Refactor to use response dto instead of no content
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPut("{listId}")]
        public async Task<IActionResult> Put(int listId, ListUpdateRequest list)
        {
            try
            {
                var updatedList = await _listService.UpdateListAsync(listId, list);
                if (updatedList == null) return NotFound();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse { Message = "Internal Server Error" });
            }
        }
        
        [HttpDelete("{listId}")]
        public async Task<IActionResult> Delete(int listId)
        {
            var success = await _listService.DeleteListAsync(listId);

            if (!success) return NotFound();

            return NoContent();
        }

        // book to list methods
        
        [HttpPost("AddBookToList")]
        public async Task<IActionResult> AddBookToList([FromBody] AddBookToListCreateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _listService.AddBookToList(
                request.ListId,
                request.BookId,
                request.IsRead,
                request.Position);
            if (result) return Ok("Book added to list successfully.");

            return BadRequest("Failed to add the book to the list.");
        }

        [HttpDelete("RemoveBookFromList")]
        public async Task<IActionResult> RemoveBookFromList([FromBody] RemoveBookFromListDeleteRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _listService.RemoveBookFromList(request.ListId, request.BookId);

            if (!result) return NotFound("Book is not in the list."); 
            
            return Ok("Book removed from list successfully.");
        }

        [HttpPost("UpdateBookPosition")]
        public async Task<IActionResult> UpdateBookPosition([FromBody] UpdateBookPositionOnListRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _listService.UpdateBookPosition(request.ListId, request.BookId, request.NewPosition);

            if (!result) return NotFound("ListBook not found or update failed.");
            
            return Ok("Book position updated successfully.");
        }
    }
}