using EventMGT.Data;
using EventMGT.DTOs;
using EventMGT.Interfaces;
using EventMGT.Models;
using Microsoft.EntityFrameworkCore;

namespace EventMGT.Repositories
{
    public class EventUserRepository : IEventUserRepository
    {
        private readonly ApplicationDbContext _context;

        public EventUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<List<EventUserDto>>> GetPagedUsersAsync(int page, int pageSize, string searchText, bool exactMatch)
        {
            // Ensure valid pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10; // Default to 10 if invalid

            var query = _context.EventUsers
                .OrderBy(u => u.Name) // Order by Name (best practice for consistency)
                .Select(user => new EventUserDto
                {
                    NIC = user.NIC,
                    Name = user.Name,
                    Department = user.Department,
                    RegistrationDate = user.RegistrationDate
                });

            if (!string.IsNullOrEmpty(searchText) && exactMatch) 
            {
                var exactRecord = await query
                    .FirstOrDefaultAsync(u => u.NIC == searchText);

                if (exactRecord != null)
                {
                    return new PagedResponse<List<EventUserDto>>(1, 1, 1, new List<EventUserDto> { exactRecord });
                }
                return new PagedResponse<List<EventUserDto>>(1, 1, 0, new List<EventUserDto>()); // No record found
            }

            // Apply search filter for partial matches
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(u => u.NIC.Contains(searchText)); // Case-sensitive search
            }

            var totalRecords = await query.CountAsync(); // Get total user count

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Fetch paginated results

            return new PagedResponse<List<EventUserDto>>(page, pageSize, totalRecords, users);
        }
    }
}
