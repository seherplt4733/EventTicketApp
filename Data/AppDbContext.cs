using Microsoft.EntityFrameworkCore;
using EventTicketApp.Models;

namespace EventTicketApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}