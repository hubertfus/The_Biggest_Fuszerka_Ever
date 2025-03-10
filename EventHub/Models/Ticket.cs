namespace EventHub
{
    public class Ticket
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public Person TicketHolder { get; set; } 

        public Ticket(string eventName, Person person)
        {
            EventName = eventName;
            TicketHolder = person;
        }

        public string PersonType => TicketHolder.PersonType;
        public override string ToString()
        {
            return $"Event: {EventName}\n" +
                   $"Buyer: {TicketHolder.Name}\n" +
                   $"Email: {TicketHolder.Email}\n" +
                   $"Type: {PersonType}";
        }
    }
}