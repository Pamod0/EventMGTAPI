using EventMGT.DTOs;
using EventMGT.Models;

namespace EventMGT.Interfaces
{
    public interface IEventUserRepository
    {
        Task<PagedResponse<List<EventUserDto>>> GetPagedUsersAsync(int page, int pageSize, string searchText, bool exactMatch);
        Task<EventUser> GetUserByNicAsync(string nic);
        Task<bool> IsUserRegisteredAsync(string nic);
        Task<EventUser> AddUserAsync(EventUser user);
        Task<EventUser> UpdateUserAsync(EventUser user);
        Task<bool> SaveChangesAsync();
    }
}