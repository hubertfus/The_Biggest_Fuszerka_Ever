using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using EventHub;

namespace EventHub
{
    public class ConnectionSettingsViewModel : INotifyPropertyChanged
    {
        private string _serverName;
        private string _databaseName;
        private bool _useIntegratedSecurity;
        private string _username;
        private string _password;
        private string _connectionStatus;

        public string ServerName
        {
            get => _serverName;
            set { _serverName = value; OnPropertyChanged(); }
        }

        public string DatabaseName
        {
            get => _databaseName;
            set { _databaseName = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set { _connectionStatus = value; OnPropertyChanged(); }
        }
        
        public ConnectionSettingsViewModel()
        {
            LoadSettingsFromEnv();
        }

        private void LoadSettingsFromEnv()
        {
            var envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
        
            if (File.Exists(envPath))
            {
                var envContent = File.ReadAllText(envPath);
                var match = Regex.Match(envContent, @"DATABASE_URL=Server=(.*?);User Id=(.*?);Password=(.*?);Database=(.*?);");

                if (match.Success)
                {
                    ServerName = match.Groups[1].Value;
                    Username = match.Groups[2].Value;
                    Password = match.Groups[3].Value;
                    DatabaseName = match.Groups[4].Value;
                }
            }
        }
        

        private void TestConnection()
        {
            var settings = new DatabaseConnectionSettings
            {
                ServerName = this.ServerName,
                DatabaseName = this.DatabaseName,
                Username = this.Username,
                Password = this.Password
            };

            if (DatabaseConnectionTester.TestConnection(settings.GetConnectionString(), out string msg))
            {
                SaveSettingsToEnv();
                ConnectionStatus = "✅ Połączenie udane!";
                var context = new EventHubContext();
                context.RefreshConnectionString(); 

                OrganizerManager.Instance.LoadOrganizers();
                EventManager.Instance.LoadEvents();
                TicketManager.Instance.LoadTickets();
            }
            else
            {
                ConnectionStatus = $"❌{msg}";
            }
        }
        
        public void Connect()
        {
            var settings = new DatabaseConnectionSettings
            {
                ServerName = this.ServerName,
                DatabaseName = this.DatabaseName,
                Username = this.Username,
                Password = this.Password
            };

            if (DatabaseConnectionTester.TestConnection(settings.GetConnectionString(), out string msg))
            {
                SaveSettingsToEnv();
                ConnectionStatus = "✅ Połączenie udane!";
                var context = new EventHubContext();
                context.RefreshConnectionString(); 
                DatabaseInitializer.Initialize(context);

                OrganizerManager.Instance.LoadOrganizers();
                EventManager.Instance.LoadEvents();
                TicketManager.Instance.LoadTickets();
            }
            else
            {
                ConnectionStatus = $"❌{msg}";
            }
        }

        private void SaveSettingsToEnv()
        {
            var envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
            var connectionString = $"DATABASE_URL=Server={ServerName};User Id={Username};Password={Password};Database={DatabaseName};";

            File.WriteAllText(envPath, connectionString);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
