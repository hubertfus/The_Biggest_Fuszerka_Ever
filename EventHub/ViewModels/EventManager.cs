using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace EventHub
{
    public class EventManager
    {
        private static EventManager _instance;
        public static EventManager Instance => _instance ??= new EventManager();

        public ObservableCollection<Event> Events { get; private set; }

        private EventManager()
        {
            using var context = new EventHubContext();
            Events = new ObservableCollection<Event>(context.Events.Include(e => e.Organizer).ToList());
        }

        public void AddEvent(Event newEvent)
        {
            using var context = new EventHubContext();
    
            if (newEvent.Date.Kind == DateTimeKind.Unspecified)
            {
                newEvent.Date = DateTime.SpecifyKind(newEvent.Date, DateTimeKind.Utc);
            }

            var existingOrganizer = context.Organizers
                .FirstOrDefault(o => o.Email == newEvent.Organizer.Email);
    
            if (existingOrganizer != null)
            {
                newEvent.Organizer = existingOrganizer;
            }
            else
            {
                context.Organizers.Add(newEvent.Organizer);
            }

            context.Events.Add(newEvent);
            context.SaveChanges();
    
            Events.Add(newEvent);
        }

        public void UpdateEvent(Event updatedEvent)
        {
            using var context = new EventHubContext();
    
            if (updatedEvent.Date.Kind == DateTimeKind.Unspecified)
            {
                updatedEvent.Date = DateTime.SpecifyKind(updatedEvent.Date, DateTimeKind.Utc);
            }
    
            context.Events.Update(updatedEvent);
            context.SaveChanges();

            var existingEvent = Events.FirstOrDefault(e => e.Id == updatedEvent.Id);
            if (existingEvent != null)
            {
                existingEvent.Name = updatedEvent.Name;
                existingEvent.Date = updatedEvent.Date;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.ImageUrl = updatedEvent.ImageUrl;
                existingEvent.Organizer = updatedEvent.Organizer;
                
                existingEvent.OnPropertyChanged(nameof(existingEvent.Name));
                existingEvent.OnPropertyChanged(nameof(existingEvent.Date));
                existingEvent.OnPropertyChanged(nameof(existingEvent.Description));
                existingEvent.OnPropertyChanged(nameof(existingEvent.ShortDescription));
                existingEvent.OnPropertyChanged(nameof(existingEvent.ImageUrl));
                existingEvent.OnPropertyChanged(nameof(existingEvent.Organizer));

            }
        }


        public void RemoveEvent(Event eventToRemove)
        {
            using var context = new EventHubContext();

            var existingEvent = context.Events
                .Include(e => e.Tickets)
                .FirstOrDefault(e => e.Id == eventToRemove.Id);

            if (existingEvent != null)
            {
                var ticketsToRemove = context.Tickets
                    .Where(t => t.EventId == existingEvent.Id)
                    .ToList();

                context.Tickets.RemoveRange(ticketsToRemove);
                context.Events.Remove(existingEvent);
                context.SaveChanges();

                foreach (var ticket in ticketsToRemove)
                {
                    var ticketInUi = TicketManager.Instance.Tickets.FirstOrDefault(t => t.Id == ticket.Id);
                    if (ticketInUi != null)
                    {
                        TicketManager.Instance.Tickets.Remove(ticketInUi);
                    }
                }

                var eventInUi = Events.FirstOrDefault(e => e.Id == eventToRemove.Id);
                if (eventInUi != null)
                {
                    Events.Remove(eventInUi);
                }
            }
        }
        public void LoadEvents()
        {
            using var context = new EventHubContext();
            var eventsFromDb = context.Events.Include(e => e.Organizer).ToList();
    
            Events.Clear();
            foreach (var eventItem in eventsFromDb)
            {
                Events.Add(eventItem);
            }
        }
    }

}
