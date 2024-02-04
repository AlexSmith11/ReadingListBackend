using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingListBackend.Requests;
using ReadingListBackend.Responses;
using ReadingListBackend.Utilities;

namespace ReadingListBackend.Interfaces
{
    public interface IListService
    {
        /// <summary>
        /// Get a single List given an Id
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<ListResponse> GetListByIdAsync(int listId);
        
        /// <summary>
        /// Get a list of all lists, NOT including the assigned books.
        /// So only a top level view of the lists - this does not return a tree structure including books, etc
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PaginatedResponse<ListSummaryResponse>> GetAllListsAsync(int page, int pageSize);
        
        /// <summary>
        /// Create a new List
        /// </summary>
        /// <param name="listCreateRequest"></param>
        /// <returns></returns>
        Task<ListResponse> CreateListAsync(ListCreateRequest listCreateRequest);
        
        /// <summary>
        /// Update a lists name
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="listUpdateRequest"></param>
        /// <returns></returns>
        Task<ListResponse> UpdateListAsync(int listId, ListUpdateRequest listUpdateRequest);
        
        /// <summary>
        /// Delete a list
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task<bool> DeleteListAsync(int listId);

        /// <summary>
        /// Add a book to a list
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="bookId"></param>
        /// <param name="isRead"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public Task<bool> AddBookToList(int listId, int bookId, bool isRead, int? position = null);
        
        /// <summary>
        /// Update a books position on a list
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="bookId"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public Task<bool> UpdateBookPosition(int listId, int bookId, int newPosition);

        /// <summary>
        /// Remove a book from a list
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public Task<bool> RemoveBookFromList(int listId, int bookId);
    }

}