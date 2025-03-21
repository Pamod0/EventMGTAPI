using EventMGT.Models;
using Microsoft.EntityFrameworkCore;

namespace EventMGT.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Member> Members { get; set; }
    }
}
