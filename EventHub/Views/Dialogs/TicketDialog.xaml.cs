using System.Windows;
using System.Windows.Controls;

namespace EventHub
{
    public partial class TicketDialog : Window
    {
        public Ticket GeneratedTicket { get; private set; }

        public TicketDialog(string eventName)
        {
            InitializeComponent();
            EventName.Text = eventName;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BuyerName.Text) || string.IsNullOrWhiteSpace(BuyerEmail.Text) || TicketType.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Person buyer;
            string selectedType = (TicketType.SelectedItem as ComboBoxItem)?.Content.ToString();

            switch (selectedType)
            {
                case "VIP":
                    buyer = new VipPerson(0, BuyerName.Text, BuyerEmail.Text);
                    break;
                case "Disabled":
                    buyer = new DisabledPerson(0, BuyerName.Text, BuyerEmail.Text);
                    break;
                default:
                    buyer = new Person(0, BuyerName.Text, BuyerEmail.Text);
                    break;
            }

            GeneratedTicket = new Ticket(EventName.Text, buyer);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}