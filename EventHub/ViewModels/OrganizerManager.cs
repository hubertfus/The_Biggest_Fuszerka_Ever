using System.Collections.ObjectModel;
using System.Windows;
using Npgsql; // Zmieniono z SqlClient na Npgsql
using DotNetEnv;

namespace EventHub
{
        public class OrganizerManager
    {
        private static OrganizerManager _instance;
        public static OrganizerManager Instance => _instance ??= new OrganizerManager();

        public ObservableCollection<Organizer> Organizers { get; private set; }

        private OrganizerManager()
        {
            Env.Load();
            using var context = new EventHubContext();
            Organizers = new ObservableCollection<Organizer>(context.Organizers.ToList());
        }

        public void AddOrganizer(Organizer newOrganizer)
        {
            using var context = new EventHubContext();
            context.Organizers.Add(newOrganizer);
            context.SaveChanges();
            Organizers.Add(newOrganizer);
        }

        public void RemoveOrganizer(Organizer organizerToRemove)
        {
            using var context = new EventHubContext();

            var eventsWithOrganizer = context.Events.Any(e => e.OrganizerId == organizerToRemove.Id);
            if (eventsWithOrganizer)
            {
                MessageBox.Show("Cannot delete the organizer because they are assigned to one or more events.",
                    "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            context.Organizers.Remove(organizerToRemove);
            context.SaveChanges();
            Organizers.Remove(organizerToRemove);
        }

        public void UpdateOrganizer(Organizer updatedOrganizer)
        {
            using var context = new EventHubContext();
            
            context.Organizers.Update(updatedOrganizer);
            context.SaveChanges();
        
            var existing = Organizers.FirstOrDefault(o => o.Id == updatedOrganizer.Id);
            if (existing != null)
            {
                existing.Name = updatedOrganizer.Name;
                existing.Email = updatedOrganizer.Email;
                existing.Description = updatedOrganizer.Description;
                existing.LogoUrl = updatedOrganizer.LogoUrl;
        
                existing.OnPropertyChanged(nameof(existing.Name));
                existing.OnPropertyChanged(nameof(existing.Email));
                existing.OnPropertyChanged(nameof(existing.Description));
                existing.OnPropertyChanged(nameof(existing.ShortDescription));
                existing.OnPropertyChanged(nameof(existing.LogoUrl));
            }
        }
        
        public void LoadOrganizers()
        {
            using var context = new EventHubContext();
            var organizersFromDb = context.Organizers.ToList();
    
            Organizers.Clear();
            foreach (var organizer in organizersFromDb)
            {
                Organizers.Add(organizer);
            }
        }

    }
}
