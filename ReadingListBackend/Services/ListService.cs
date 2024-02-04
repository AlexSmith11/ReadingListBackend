using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReadingListBackend.Database;
using ReadingListBackend.Exceptions;
using ReadingListBackend.Interfaces;
using ReadingListBackend.Models;
using ReadingListBackend.Requests;
using ReadingListBackend.Responses;
using ReadingListBackend.Utilities;

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

        public async Task<PaginatedResponse<ListSummaryResponse>> GetAllListsAsync(int page = 1, int pageSize = 10)
        {
            var query = _context.Lists
                .Select(list => new ListSummaryResponse
                {
                    Id = list.Id,
                    Name = list.Name
                });

            var totalItems = await query.CountAsync();
            var lists = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<ListSummaryResponse>
            {
                Items = lists,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<ListResponse> GetListByIdAsync(int listId)
        {
            var list = await _context.Lists
                .Include(l => l.ListBooks)
                .ThenInclude(lb => lb.Book)
                .ThenInclude(b => b.Author)
                .Include(l => l.ListBooks)
                .ThenInclude(lb => lb.Book)
                .ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                throw new NotFoundException("List not found");
            }

            return _mapper.Map<ListResponse>(list);
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

        public async Task<ListResponse> UpdateListAsync(int listId, ListUpdateRequest listUpdateRequest)
        {
            var existingList = await _context.Lists.FindAsync(listId);
            if (existingList == null) throw new NotFoundException("List not found");

            existingList.Name = listUpdateRequest.Name;

            _context.Entry(existingList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListExists(listId))
                    return null;
                throw;
            }

            return _mapper.Map<ListResponse>(existingList);
        }

        private bool ListExists(int listId)
        {
            return _context.Lists.Any(l => l.Id == listId);
        }
        
        public async Task<bool> DeleteListAsync(int listId)
        {
            var list = await _context.Lists.FindAsync(listId);
            if (list == null)
                return false;

            _context.Lists.Remove(list);
            await _context.SaveChangesAsync();

            return true;
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