namespace EventHub.Seeders;

using Microsoft.EntityFrameworkCore;

public static class OrganizerSeeder
{
    public static void Seed(EventHubContext context)
    {
        if (context.Organizers.Any()) return; 

        var organizers = new List<Organizer>
        {
            new Organizer
            {
                Name = "Tech Events Inc.",
                Description = "Leading organizer of technology conferences and workshops across the country. We bring together the brightest minds in tech to share knowledge and network.",
                Email = "info@techevents.com",
                LogoUrl = "https://picsum.photos/200/300"
            },
            new Organizer
            {
                Name = "Music Festivals Worldwide",
                Description = "Organizers of the biggest music festivals in Europe and North America. Specializing in multi-genre festivals with international artists.",
                Email = "contact@musicfest.com",
                LogoUrl = "https://picsum.photos/200/300"
            },
            new Organizer
            {
                Name = "Business Summit Group",
                Description = "Professional business conferences for executives and entrepreneurs. Our events focus on innovation, leadership, and market trends.",
                Email = "events@businesssummit.com",
                LogoUrl = "https://picsum.photos/200/300"
            }
        };

        context.Organizers.AddRange(organizers);
        context.SaveChanges();
    }
}