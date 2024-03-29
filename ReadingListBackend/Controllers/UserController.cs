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
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<UserResponse>>> GetUsers(int page = 1, int pageSize = 10)
        {
            var query = _context.Users.AsQueryable();
            var totalItems = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<UserResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var response = new PaginatedResponse<UserResponse>
            {
                Items = users,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> Get(int id)
        {
            var user  = await _context.Users
                .Where(b => b.Id == id)
                .ProjectTo<UserResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            
            if (user == null) return NotFound();

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create([FromBody] UserCreateRequest userRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User
            {
                Username = userRequest.Username,
                Email = userRequest.Email,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            var userResponse = _mapper.Map<UserResponse>(user);

            return CreatedAtAction(nameof(Get), new {id = user.Id}, userResponse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UserUpdateRequest updateUserRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Update the user
            if (!string.IsNullOrEmpty(updateUserRequest.Username)) user.Username = updateUserRequest.Username;
            if (!string.IsNullOrEmpty(updateUserRequest.Email)) user.Email = updateUserRequest.Email;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) return NotFound();
                else throw;
            }
            
            var userAuthor = _mapper.Map<AuthorResponse>(user);

            return Ok(userAuthor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}