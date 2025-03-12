using System.ComponentModel;

namespace EventHub
{
    public class Organizer : INotifyPropertyChanged
    {
        private string? _name;
        private string? _description;
        private string? _email;
        private string? _logoUrl;
        
        public int Id {get; set;}
        public string? Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string? Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string? LogoUrl
        {
            get => _logoUrl;
            set
            {
                if (_logoUrl != value)
                {
                    _logoUrl = value;
                    OnPropertyChanged(nameof(LogoUrl));
                }
            }
        }

        public override string ToString()
        {
            return $"Id: {Id}\n Name: {Name} \n Description: {Description} \n Email: {Email}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}