namespace EventHub.Seeders;

using Microsoft.EntityFrameworkCore;

public static class EventSeeder
{
    public static void Seed(EventHubContext context)
    {
        if (context.Events.Any()) return;

        var organizers = context.Organizers.ToList();
        if (!organizers.Any()) return;

        var events = new List<Event>
        {
            new Event
            {
                Name = "Future Tech Conference 2023",
                Date = DateTime.UtcNow.AddMonths(2), // Changed to UTC
                Description = "Join us for the annual Future Tech Conference...",
                ImageUrl = "https://example.com/images/future-tech.jpg",
                OrganizerId = organizers[0].Id
            },
            new Event
            {
                Name = "Summer Music Festival",
                Date = DateTime.UtcNow.AddMonths(3), // Changed to UTC
                Description = "The biggest music festival of the year...",
                ImageUrl = "https://example.com/images/summer-fest.jpg",
                OrganizerId = organizers[1].Id
            },
            new Event
            {
                Name = "Business Innovation Summit",
                Date = DateTime.UtcNow.AddMonths(1), // Changed to UTC
                Description = "A two-day summit focusing on innovative business strategies...",
                ImageUrl = "https://example.com/images/business-summit.jpg",
                OrganizerId = organizers[2].Id
            }
        };

        context.Events.AddRange(events);
        context.SaveChanges();
    }
}