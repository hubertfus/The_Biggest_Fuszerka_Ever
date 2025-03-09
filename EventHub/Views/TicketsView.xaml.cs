using System.Windows;
using System.Windows.Controls;

namespace EventHub
{
    public partial class TicketsView : UserControl
    {
        public TicketsView()
        {
            InitializeComponent();
            DataContext = TicketManager.Instance;
        }

        private void DeleteTicket_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Ticket ticket)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete the ticket for {ticket.TicketHolder.Name}?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    TicketManager.Instance.RemoveTicket(ticket);
                }
            }
        }
    }
}