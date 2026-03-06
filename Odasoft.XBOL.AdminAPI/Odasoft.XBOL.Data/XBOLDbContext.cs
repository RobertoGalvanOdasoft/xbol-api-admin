using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Odasoft.XBOL.Data.Configurations;
using Odasoft.XBOL.Data.Extensions;
using Odasoft.XBOL.Models;

namespace Odasoft.XBOL.Data
{
    public class XBOLDbContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Season> Seasons => Set<Season>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<SeasonPass> SeasonPasses => Set<SeasonPass>();
        public DbSet<SeasonPassEventTicket> SeasonPassEventTickets => Set<SeasonPassEventTicket>();
        public DbSet<BaseSeat> BaseSeats => Set<BaseSeat>();
        public DbSet<BaseRow> BaseRows => Set<BaseRow>();
        public DbSet<BaseSection> BaseSections => Set<BaseSection>();
        public DbSet<BaseZone> BaseZones => Set<BaseZone>();
        public DbSet<VenueMap> VenueMaps => Set<VenueMap>();
        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<EventSeat> EventSeats => Set<EventSeat>();
        public DbSet<EventSection> EventSections => Set<EventSection>();
        public DbSet<EventSchedule> EventSchedules => Set<EventSchedule>();
        public DbSet<SuiteLevel> SuiteLevels => Set<SuiteLevel>();
        public DbSet<Suite> Suites => Set<Suite>();
        public DbSet<SuiteAgreement> SuiteAgreements => Set<SuiteAgreement>();
        public DbSet<SuiteAgreementFile> SuiteAgreementFiles => Set<SuiteAgreementFile>();
        public DbSet<InventoryBatch> InventoryBatches => Set<InventoryBatch>();
        public DbSet<Performer> Performers => Set<Performer>();
        public DbSet<SeasonSection> SeasonSections => Set<SeasonSection>();
        public DbSet<SeasonSeat> SeasonSeats => Set<SeasonSeat>();
        public DbSet<ClientCreditAccount> ClientCreditAccounts => Set<ClientCreditAccount>();
        public DbSet<ClientCreditTransaction> ClientCreditTransactions => Set<ClientCreditTransaction>();
        public DbSet<LegalRepresentative> LegalRepresentatives => Set<LegalRepresentative>();
        public DbSet<SequenceTracker> SequenceTrackers => Set<SequenceTracker>();
        public DbSet<PhoneRegionCode> PhoneRegionCodes => Set<PhoneRegionCode>();

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
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(BaseModel.Id))
                        .ValueGeneratedOnAdd();
                }
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithOne(u => u.Client)
                .HasForeignKey<User>(u => u.ClientId)
                .IsRequired();

            modelBuilder.Entity<SeasonPassEventTicket>()
                .Property(spet => spet.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new ClientCreditAccountConfiguration());
            modelBuilder.ApplyConfiguration(new ClientCreditTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new LegalRepresentativeConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizerConfiguration());
            modelBuilder.ApplyConfiguration(new SuiteAgreementConfiguration());
            modelBuilder.ApplyConfiguration(new SuiteAgreementFileConfiguration());
            modelBuilder.ApplyConfiguration(new SuiteConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new VenueConfiguration());
        }
    }
}
