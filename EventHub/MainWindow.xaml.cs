using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace EventHub
{
    public partial class MainWindow : Window
    {
        private bool _isInitialized = false;


        public MainWindow()
        {
            InitializeComponent();
            using (var context = new EventHubContext())
            {
                context.Database.Migrate();
            }
            ViewController.RegisterView("HomeView", new HomeView());
            ViewController.SetMainContentArea(MainContentArea);
            
            _isInitialized = true;
            
            ViewController.ChangeView("HomeView");
            ViewController.RegisterView("EventsView", new EventsView());
            ViewController.RegisterView("AboutView", new AboutView());
            ViewController.RegisterView("OrganizersView", new OrganizersView());
            ViewController.RegisterView("TicketsView", new TicketsView());
            ViewController.RegisterView("ConnectionSettingsView", new ConnectionSettingsView());
            
        }

        private void MenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized)
                return;
            if (sender is ListBox listBox && listBox.SelectedItem is ListBoxItem selectedItem)
            {
                string selectedView = selectedItem.Tag.ToString();
                ViewController.ChangeView(selectedView);
            }
        }
    }
}