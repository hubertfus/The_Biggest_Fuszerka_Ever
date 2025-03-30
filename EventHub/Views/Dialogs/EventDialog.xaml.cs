using System;
using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;

namespace EventHub
{
    public partial class EventDialog : Window
    {
        public Event EventItem { get; private set; }

        public EventDialog(Event eventToEdit = null)
        {
            InitializeComponent();
            LoadOrganizers();

            if (eventToEdit != null)
            {
                EventItem = eventToEdit;
                EventName.Text = EventItem.Name;
                EventDate.Text = EventItem.Date.ToString("yyyy-MM-dd"); // Konwersja DateTime do string
                EventDescription.Text = EventItem.Description;
                EventImageUrl.Text = EventItem.ImageUrl;
                if (eventToEdit.OrganizerId > 0)
                {
                    var selectedOrganizer = OrganizerManager.Instance.Organizers
                        .FirstOrDefault(o => o.Id == eventToEdit.OrganizerId);
                    if (selectedOrganizer != null)
                    {
                        OrganizerList.SelectedItem = selectedOrganizer;
                    }
                }
            }
            else
            {
                EventItem = new Event();
            }
        }

        private void LoadOrganizers()
        {
            OrganizerList.ItemsSource = OrganizerManager.Instance.Organizers;
            OrganizerList.DisplayMemberPath = "Name";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            EventItem.Name = EventName.Text;

            // Konwersja stringa do DateTime
            if (DateTime.TryParse(EventDate.Text, out DateTime parsedDate))
            {
                EventItem.Date = parsedDate;
            }
            else
            {
                MessageBox.Show("Invalid date format! Use YYYY-MM-DD.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EventItem.Description = EventDescription.Text;
            EventItem.ImageUrl = EventImageUrl.Text;
            EventItem.Organizer = OrganizerList.SelectedItem as Organizer;

            DialogResult = true;
            Close();
        }

        private bool ValidateFields()
        {
            Console.WriteLine(EventDate.Text);
            if (string.IsNullOrWhiteSpace(EventName.Text) ||
                string.IsNullOrWhiteSpace(EventDate.Text) ||
                string.IsNullOrWhiteSpace(EventDescription.Text) ||
                string.IsNullOrWhiteSpace(EventImageUrl.Text) ||
                OrganizerList.SelectedItem == null)
            {   
                MessageBox.Show("All fields must be filled!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!DateTime.TryParse(EventDate.Text, out _))
            {
                MessageBox.Show("Invalid date format! Use YYYY.MM.DD.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
