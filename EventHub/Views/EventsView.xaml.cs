using System.Windows;
using System.Windows.Controls;

namespace EventHub
{
    public partial class EventsView : UserControl
    {
        public EventsView()
        {
            InitializeComponent();
            DataContext = EventManager.Instance;
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            EventDialog dialog = new EventDialog(null); 
            dialog.ShowDialog();

            if (dialog.DialogResult == true)
            {
                EventManager.Instance.AddEvent(dialog.EventItem);
            }
        }

        private void EditEvent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Event eventItem)
            {
                EventDialog dialog = new EventDialog(eventItem); 
                dialog.ShowDialog();

                if (dialog.DialogResult == true)
                {
                    EventManager.Instance.RemoveEvent(eventItem);
                    EventManager.Instance.AddEvent(dialog.EventItem);
                }
            }
        }

        private void GenerateTicket_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Event eventItem)
            {
                TicketDialog dialog = new TicketDialog(eventItem.Name);
                dialog.ShowDialog();

                if (dialog.DialogResult == true)
                {
                    Ticket ticket = dialog.GeneratedTicket;
                    Console.WriteLine(ticket.TicketHolder.GetPersonType());
                    TicketManager.Instance.AddTicket(ticket);

                    MessageBox.Show($"Ticket Generated!\n\n{ticket}", 
                        "Ticket Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        
        private void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Event eventItem)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete '{eventItem.Name}'?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    EventManager.Instance.RemoveEvent(eventItem);
                }
            }
        }
    }
}
