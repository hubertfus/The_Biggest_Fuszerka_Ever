using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace EventHub
{
    public partial class ConnectionSettingsView : UserControl
    {
        public ConnectionSettingsView()
        {
            InitializeComponent();
            this.DataContext = new ConnectionSettingsViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ConnectionSettingsViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }

        private void Test_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ConnectionSettingsViewModel vm)
            {
                var settings = new DatabaseConnectionSettings
                {
                    ServerName = vm.ServerName,
                    DatabaseName = vm.DatabaseName,
                    Username = vm.Username,
                    Password = vm.Password
                };

                if (DatabaseConnectionTester.TestConnection(settings.GetConnectionString(), out string msg))
                {
                    string connectionString = $"Server={settings.ServerName};User Id={settings.Username};Password={settings.Password};Database={settings.DatabaseName};";
                    Environment.SetEnvironmentVariable("DATABASE_URL", connectionString, EnvironmentVariableTarget.Process);
                    vm.ConnectionStatus = $"{msg}";
                }
                else
                {
                    vm.ConnectionStatus = $"{msg}";
                }
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ConnectionSettingsViewModel vm)
            {
                var settings = new DatabaseConnectionSettings
                {
                    ServerName = vm.ServerName,
                    DatabaseName = vm.DatabaseName,
                    Username = vm.Username,
                    Password = vm.Password
                };

                if (DatabaseConnectionTester.TestConnection(settings.GetConnectionString(), out string msg))
                {
                    string connectionString = $"Server={settings.ServerName};User Id={settings.Username};Password={settings.Password};Database={settings.DatabaseName};";
                    Environment.SetEnvironmentVariable("DATABASE_URL", connectionString, EnvironmentVariableTarget.Process);

                    vm.ConnectionStatus = "✅ Połączenie udane i zapisane w zmiennej środowiskowej.";
                    
                    vm.Connect();
                }
                else
                {
                    vm.ConnectionStatus = $"{msg}";
                }
            }
        }

    }


}