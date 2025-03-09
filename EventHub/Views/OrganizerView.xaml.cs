using System.Windows;
using System.Windows.Controls;

namespace EventHub
{
    public partial class OrganizersView : UserControl
    {
        public OrganizersView()
        {
            InitializeComponent();
            DataContext = OrganizerManager.Instance;
        }

        private void AddOrganizer_Click(object sender, RoutedEventArgs e)
        {
            // Open the dialog for adding a new organizer
            OrganizerDialog organizerDialog = new OrganizerDialog();
            bool? dialogResult = organizerDialog.ShowDialog();

            // If dialog result is true (Save button clicked), add the new organizer
            if (dialogResult == true)
            {
                OrganizerManager.Instance.AddOrganizer(organizerDialog.Organizer);
            }
        }

        private void DeleteOrganizer_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var organizerToRemove = (Organizer)button.DataContext;
            OrganizerManager.Instance.RemoveOrganizer(organizerToRemove);
        }

        private void EditOrganizer_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var organizerToEdit = (Organizer)button.DataContext;

            OrganizerDialog organizerDialog = new OrganizerDialog(organizerToEdit);
            bool? dialogResult = organizerDialog.ShowDialog();

            
        }
    }
}