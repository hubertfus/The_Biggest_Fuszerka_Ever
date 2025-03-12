using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace EventHub
{
    public partial class TicketDialog : Window
    {
        public Ticket GeneratedTicket { get; private set; }
        private Event EventData;
        public TicketDialog(Event eventData)
        {
            InitializeComponent();
            EventData = eventData;
            EventName.Text = eventData.Name;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
            {
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

            GeneratedTicket = new Ticket(EventData, buyer);
            DialogResult = true;
            Close();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(BuyerName.Text))
            {
                MessageBox.Show("Buyer name cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(BuyerEmail.Text) || !IsValidEmail(BuyerEmail.Text))
            {
                MessageBox.Show("Invalid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (TicketType.SelectedItem == null)
            {
                MessageBox.Show("Please select a ticket type.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
