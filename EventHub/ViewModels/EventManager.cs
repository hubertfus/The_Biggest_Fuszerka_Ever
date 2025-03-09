using System.Collections.ObjectModel;

namespace EventHub
{
    public class EventManager
    {
        private static EventManager _instance;
        public static EventManager Instance => _instance ??= new EventManager();

        public ObservableCollection<Event> Events { get; private set; }

        private EventManager()
        {
            Events = new ObservableCollection<Event>
            {
                new Event { Name = "Tech Conference 2025", Date = "March 15, 2025", 
                    Description = "A conference about the latest trends in technology.", 
                    ImageUrl = "https://picsum.photos/150/100" },

                new Event { Name = "Music Festival", Date = "April 22, 2025", 
                    Description = "Enjoy live performances from top artists.", 
                    ImageUrl = "https://picsum.photos/150/100" },

                new Event { Name = "Startup Meetup", Date = "May 10, 2025", 
                    Description = "Networking and pitching opportunities for startups.", 
                    ImageUrl = "https://picsum.photos/150/100" }
            };
        }

        public void AddEvent(Event newEvent)
        {
            Events.Add(newEvent);
        }

        public void RemoveEvent(Event eventToRemove)
        {
            Events.Remove(eventToRemove);
        }
    }
}