using System.Collections.ObjectModel;

namespace EventHub
{
    public class TicketManager
    {
        private static TicketManager _instance;
        public static TicketManager Instance => _instance ??= new TicketManager();

        public ObservableCollection<Ticket> Tickets { get; private set; }

        private TicketManager()
        {
            Tickets = new ObservableCollection<Ticket>();
        }

        public void AddTicket(Ticket ticket)
        {
            Tickets.Add(ticket);
        }

        public void RemoveTicket(Ticket ticket)
        {
            Tickets.Remove(ticket);
        }
    }
}