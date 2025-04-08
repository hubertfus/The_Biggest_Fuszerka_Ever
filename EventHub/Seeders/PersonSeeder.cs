namespace EventHub.Seeders;

using Microsoft.EntityFrameworkCore;

public static class PersonSeeder
{
    public static void Seed(EventHubContext context)
    {
        if (context.People.Any()) return; // DB already seeded

        var people = new List<Person>
        {
            new Person
            {
                Name = "John Smith",
                Email = "john.smith@example.com",
                PersonType = "Standard"
            },
            new VipPerson
            {
                Name = "Emma Johnson",
                Email = "emma.johnson@example.com"
            },
            new DisabledPerson
            {
                Name = "Michael Brown",
                Email = "michael.brown@example.com"
            },
            new Person
            {
                Name = "Sarah Williams",
                Email = "sarah.williams@example.com",
                PersonType = "Standard"
            },
            new VipPerson
            {
                Name = "David Miller",
                Email = "david.miller@example.com"
            }
        };

        context.People.AddRange(people);
        context.SaveChanges();
    }
}