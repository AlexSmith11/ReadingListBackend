using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Database;
using ReadingListBackend.Exceptions;
using ReadingListBackend.Interfaces;
using ReadingListBackend.Models;
using ReadingListBackend.Requests;
using ReadingListBackend.Responses;

namespace ReadingListBackend.Services
{
    public class ListService : IListService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ListService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<ListResponse> CreateListAsync(ListCreateRequest listCreateRequest)
        {
            var user = await _context.Users.FindAsync(listCreateRequest.UserId);
            if (user == null) throw new NotFoundException("User not found");
            
            var newList = _mapper.Map<List>(listCreateRequest);
            
            await _context.Lists.AddAsync(newList);
            await _context.SaveChangesAsync();

            return _mapper.Map<ListResponse>(newList);
        }

        /// <summary>
        /// Create a new list in the database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> CreateListAsync(string name, int userId)
        {
            
        }

        public async Task<bool> AddBookToList(int listId, int bookId, bool isRead, int? position = null)
        {
            var list = await _context.Lists.FindAsync(listId);
            var book = await _context.Books.FindAsync(bookId);

            // check if List and Book exist
            if (list == null || book == null) return false;

            position ??= list.ListBooks.Count + 1;

            var userListBook = new ListBook
            {
                ListId = listId,
                BookId = bookId,
                IsRead = isRead,
                Position = position.Value
            };

            await _context.ListBooks.AddAsync(userListBook);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateBookPosition(int listId, int bookId, int newPosition)
        {
            // Find the ListBook entry to update
            var listBook = await _context.ListBooks
                .FirstOrDefaultAsync(ulb => ulb.ListId == listId && ulb.BookId == bookId);

            if (listBook == null) return false;

            // Update the position
            listBook.Position = newPosition;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveBookFromList(int listId, int bookId)
        {
            var listBook = await _context.ListBooks
                .FirstOrDefaultAsync(ulb => ulb.ListId == listId && ulb.BookId == bookId);

            if (listBook == null)
            {
                // Book is not in the list
                return false;
            }

            // Remove the ListBook entry
            _context.ListBooks.Remove(listBook);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}