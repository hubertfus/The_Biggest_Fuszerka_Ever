namespace EventHub
{
    public class Ticket
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public int TicketHolderId { get; set; }
        public Person TicketHolder { get; set; }

        public string PersonType => TicketHolder?.PersonType ?? "Unknown";

        public Ticket() { }

        public Ticket(Event xevent, Person person)
        {
            Event = xevent;
            TicketHolder = person;
        }

        public override string ToString()
        {
            return $"Event: {Event?.Name}\n" +
                   $"Buyer: {TicketHolder?.Name}\n" +
                   $"Email: {TicketHolder?.Email}\n" +
                   $"Type: {PersonType}";
        }
    }
}