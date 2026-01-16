using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Configurations;
using Odasoft.XBOL.Data.Extensions;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data
{
    public class XBOLDbContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<EventSeat> EventSeats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<EventSchedule> EventSchedules { get; set; }

        public XBOLDbContext() : base()
        {
        }

        public XBOLDbContext(DbContextOptions<XBOLDbContext> options)
           : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(@"Host=localhost;Port=5432;Database=XBOL;Username=postgres;Password=12345");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.ApplyConfiguration(new TicketConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
