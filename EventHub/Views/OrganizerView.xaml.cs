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
            OrganizerDialog organizerDialog = new OrganizerDialog();
            bool? dialogResult = organizerDialog.ShowDialog();

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
            if (sender is Button button && button.DataContext is Organizer organizerItem)
            {
                string originalName = organizerItem.Name;

                OrganizerDialog dialog = new OrganizerDialog(new Organizer
                {
                    Id = organizerItem.Id,
                    Name = organizerItem.Name,
                    Email = organizerItem.Email,
                    Description = organizerItem.Description,
                    LogoUrl = organizerItem.LogoUrl
                });
                dialog.ShowDialog();

                if (dialog.DialogResult == true)
                {
                    OrganizerManager.Instance.UpdateOrganizer(dialog.Organizer);
                }
            }
        }    }
}