using EventHub.Seeders;

namespace EventHub;

using Microsoft.EntityFrameworkCore;

public static class DatabaseInitializer
{
    public static void Initialize(EventHubContext context)
    {
        context.Database.Migrate();

        OrganizerSeeder.Seed(context);
        EventSeeder.Seed(context);
        PersonSeeder.Seed(context);
        TicketSeeder.Seed(context);
    }
}