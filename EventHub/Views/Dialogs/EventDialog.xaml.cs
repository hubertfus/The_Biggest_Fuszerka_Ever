using System.Windows;

namespace EventHub
{
    public partial class EventDialog : Window
    {
        public Event EventItem { get; private set; }

        public EventDialog(Event eventToEdit)
        {
            InitializeComponent();

            if (eventToEdit != null)
            {
                EventItem = eventToEdit;
                EventName.Text = EventItem.Name;
                EventDate.Text = EventItem.Date;
                EventDescription.Text = EventItem.Description;
                EventImageUrl.Text = EventItem.ImageUrl;
            }
            else
            {
                EventItem = new Event();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            EventItem.Name = EventName.Text;
            EventItem.Date = EventDate.Text;
            EventItem.Description = EventDescription.Text;
            EventItem.ImageUrl = EventImageUrl.Text;

            DialogResult = true;
            Close();
        }

        // Cancel the dialog
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}