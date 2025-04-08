using System.Collections.ObjectModel;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace EventHub
{
    public class TicketManager
    {
        private static TicketManager _instance;
        public static TicketManager Instance => _instance ??= new TicketManager();

        public ObservableCollection<Ticket> Tickets { get; private set; }

        private TicketManager()
        {
            Env.Load();
            using var context = new EventHubContext();
            var tickets = context.Tickets
                .Include(t => t.Event)
                .Include(t => t.TicketHolder)
                .ToList();

            Tickets = new ObservableCollection<Ticket>(tickets);
        }

        public void AddTicket(Ticket ticket)
        {
            using var context = new EventHubContext();

            var existingPerson = context.People
                .FirstOrDefault(p => p.Name == ticket.TicketHolder.Name && p.Email == ticket.TicketHolder.Email);

            if (existingPerson == null)
            {
                context.People.Add(ticket.TicketHolder);
                context.SaveChanges();
            }
            else
            {
                ticket.TicketHolder = existingPerson;
            }

            var existingEvent = context.Events.Find(ticket.Event.Id);
            if (existingEvent != null)
            {
                ticket.Event = existingEvent;
            }

            context.Tickets.Add(ticket);
            context.SaveChanges();

            Tickets.Add(ticket);
        }




        public void RemoveTicket(Ticket ticket)
        {
            using var context = new EventHubContext();
            var existingTicket = context.Tickets
                .Include(t => t.TicketHolder)
                .FirstOrDefault(t => t.Id == ticket.Id);

            if (existingTicket != null)
            {
                context.Tickets.Remove(existingTicket);
                context.SaveChanges();

                var ticketHolderId = existingTicket.TicketHolder.Id;

                var hasOtherTickets = context.Tickets
                    .Any(t => t.TicketHolder.Id == ticketHolderId);

                if (!hasOtherTickets)
                {
                    var orphanPerson = context.People
                        .Include(p => p.Tickets)
                        .FirstOrDefault(p => p.Id == ticketHolderId);

                    if (orphanPerson != null)
                    {
                        context.People.Remove(orphanPerson);
                        context.SaveChanges();
                    }
                }

                Tickets.Remove(ticket);
            }
        }
        public void LoadTickets()
        {
            using var context = new EventHubContext();
            var ticketsFromDb = context.Tickets
                .Include(t => t.Event)
                .Include(t => t.TicketHolder)
                .ToList();

            Tickets.Clear();
            foreach (var ticket in ticketsFromDb)
            {
                Tickets.Add(ticket);
            }
        }

    }
}
