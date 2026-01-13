using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Extensions;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data
{
    public class XBOLDbContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<EventSeat> EventSeats { get; set; }

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
                optionsBuilder.UseNpgsql(@"Host=localhost;Port=5432;Database=XBOL;Username=postgres;Password=OdaUser@123!");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.RemovePluralizingTableNameConvention();
        }
    }
}
