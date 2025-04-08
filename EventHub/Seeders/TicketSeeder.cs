namespace EventHub.Seeders;

using Microsoft.EntityFrameworkCore;

public static class TicketSeeder
{
    public static void Seed(EventHubContext context)
    {
        if (context.Tickets.Any()) return; // DB already seeded

        var events = context.Events.ToList();
        var people = context.People.ToList();

        if (!events.Any() || !people.Any()) return; // Need events and people first

        var tickets = new List<Ticket>
        {
            new Ticket(events[0], people[0]),
            new Ticket(events[0], people[1]),
            new Ticket(events[1], people[2]),
            new Ticket(events[1], people[3]),
            new Ticket(events[2], people[4]),
            new Ticket(events[2], people[0])
        };

        context.Tickets.AddRange(tickets);
        context.SaveChanges();
    }
}