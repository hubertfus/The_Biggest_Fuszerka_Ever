using Microsoft.EntityFrameworkCore;

namespace EventHub
{
    public class EventHubContext : DbContext
    {
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Organizer> Organizers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DotNetEnv.Env.Load();
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            optionsBuilder.UseNpgsql(connectionString);
            Console.WriteLine(connectionString);
        }
        
        public void RefreshConnectionString()
        {
            DotNetEnv.Env.Load();
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            this.Database.SetConnectionString(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasDiscriminator<string>("PersonType")
                .HasValue<Person>("Standard")
                .HasValue<VipPerson>("VIP")
                .HasValue<DisabledPerson>("Disabled");

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.TicketHolder)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.TicketHolderId);
        }
    }
}