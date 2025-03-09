using System.Windows;
using System.Windows.Controls;

namespace EventHub
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewController.SetMainContentArea(MainContentArea);

            ViewController.RegisterView("HomeView", new HomeView());
            ViewController.RegisterView("EventsView", new EventsView());
            ViewController.RegisterView("AboutView", new AboutView());
            ViewController.RegisterView("OrganizersView", new OrganizersView());
            ViewController.RegisterView("TicketsView", new TicketsView());

            ViewController.ChangeView("HomeView");
        }

        private void MenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is ListBoxItem selectedItem)
            {
                string selectedView = selectedItem.Tag.ToString();
                ViewController.ChangeView(selectedView);
            }
        }
    }
}