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
            Organizer.Name = OrganizerName.Text;
            Organizer.Description = OrganizerDescription.Text;
            Organizer.Email = OrganizerEmail.Text;
            Organizer.LogoUrl = OrganizerLogoUrl.Text;

            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}