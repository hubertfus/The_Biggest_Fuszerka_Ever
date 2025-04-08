namespace EventHub
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PersonType { get; set; }

        public Person() { } 
        
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>(); 


        public Person(int id, string name, string email, string personType = "Standard")
        {
            Id = id;
            Name = name;
            Email = email;
            PersonType = personType;
        }
    }

    public class VipPerson : Person
    {
        public VipPerson() { }

        public VipPerson(int id, string name, string email)
            : base(id, name, email, "VIP") { }
    }

    public class DisabledPerson : Person
    {
        public DisabledPerson() { }

        public DisabledPerson(int id, string name, string email)
            : base(id, name, email, "Disabled") { }
    }
}