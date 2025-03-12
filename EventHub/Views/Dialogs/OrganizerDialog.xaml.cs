using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace EventHub
{
    public partial class OrganizerDialog : Window
    {
        public Organizer Organizer { get; set; }

        public OrganizerDialog()
        {
            InitializeComponent();
            Organizer = new Organizer(); 
            DataContext = Organizer;
        }

        public OrganizerDialog(Organizer organizer) : this()
        {
            Organizer = organizer;
            OrganizerName.Text = organizer.Name;
            OrganizerDescription.Text = organizer.Description;
            OrganizerEmail.Text = organizer.Email;
            OrganizerLogoUrl.Text = organizer.LogoUrl;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            Organizer.Name = OrganizerName.Text;
            Organizer.Description = OrganizerDescription.Text;
            Organizer.Email = OrganizerEmail.Text;
            Organizer.LogoUrl = OrganizerLogoUrl.Text;

            this.DialogResult = true;
            this.Close();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(OrganizerName.Text) || string.IsNullOrWhiteSpace(OrganizerEmail.Text) )
            {
                MessageBox.Show("All fields must be filled!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            
            if (!IsValidEmail(OrganizerEmail.Text))
            {
                MessageBox.Show("The provided email address is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            this.DialogResult = false;
            this.Close();
        }
    }
}
