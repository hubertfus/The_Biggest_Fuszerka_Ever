namespace EventHub
{
    public class Person
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public Person(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public virtual string GetPersonType()
        {
            return "Standard";
        }
    }

    public class VipPerson : Person
    {
        public VipPerson(string name, string email) : base(name, email) { }

        public override string GetPersonType()
        {
            return "VIP";
        }
    }

    public class DisabledPerson : Person
    {
        public DisabledPerson(string name, string email) : base(name, email) { }

        public override string GetPersonType()
        {
            return "Disabled";
        }
    }
}