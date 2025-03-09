using System.Windows;
using System.Windows.Controls;

namespace EventHub;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }
    
    private void ExploreEvents_Click(object sender, RoutedEventArgs e)
    {
        ViewController.ChangeView("EventsView");
    }
}