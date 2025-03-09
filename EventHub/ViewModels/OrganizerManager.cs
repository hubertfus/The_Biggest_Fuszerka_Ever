using System.Collections.ObjectModel;

namespace EventHub
{
    public class OrganizerManager
    {
        private static OrganizerManager _instance;
        public static OrganizerManager Instance => _instance ??= new OrganizerManager();

        public ObservableCollection<Organizer> Organizers { get; private set; }

        private OrganizerManager()
        {
            Organizers = new ObservableCollection<Organizer>
            {
                new Organizer { Name = "John Doe", Email = "johndoe@email.com",
                    Description  = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque euismod.", 
                    LogoUrl = "https://picsum.photos/150/100" },
                new Organizer { Name = "Jane Smith", Email = "janesmith@email.com",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque euismod.", 
                    LogoUrl = "https://picsum.photos/150/100" }
            };
        }

        public void AddOrganizer(Organizer newOrganizer)
        {
            Organizers.Add(newOrganizer);
        }

        public void RemoveOrganizer(Organizer organizerToRemove)
        {
            Organizers.Remove(organizerToRemove);
        }
    }


}