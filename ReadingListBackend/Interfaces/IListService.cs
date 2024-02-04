using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingListBackend.Requests;
using ReadingListBackend.Responses;

namespace ReadingListBackend.Interfaces
{
    public interface IListService
    {
        Task<ListResponse> GetListAsync(int listId);
        Task<IEnumerable<ListSummaryResponse>> GetAllListsAsync();
        Task<ListResponse> CreateListAsync(ListCreateRequest listCreateRequest);
    }

}