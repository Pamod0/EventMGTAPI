using EventMGT.DTOs;
using EventMGT.Models;

namespace EventMGT.Interfaces
{
    public interface IEventUserRepository
    {
        Task<PagedResponse<List<EventUserDto>>> GetPagedUsersAsync(int page, int pageSize, string searchText, bool exactMatch);
    }
}
