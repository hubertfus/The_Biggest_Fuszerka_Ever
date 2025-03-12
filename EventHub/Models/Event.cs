using System;
using System.ComponentModel;
using System.Linq;
using EventHub;

public class Event : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private int? _organizerId;

    private string _name;
    private DateTime _date;
    private string _description;
    private string _imageUrl;
    private Organizer _organizer;

    public int Id { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public DateTime Date  // Zmieniono typ z string na DateTime
    {
        get => _date;
        set
        {
            _date = value;
            OnPropertyChanged(nameof(Date));
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    public string ImageUrl
    {
        get => _imageUrl;
        set
        {
            _imageUrl = value;
            OnPropertyChanged(nameof(ImageUrl));
        }
    }

    public int? OrganizerId
    {
        get => _organizerId;
        set
        {
            if (_organizerId != value)
            {
                _organizerId = value;
                OnPropertyChanged(nameof(OrganizerId));

                if (_organizerId.HasValue)
                {
                    Organizer = OrganizerManager.Instance.Organizers.FirstOrDefault(o => o.Id == _organizerId.Value);
                }
                else
                {
                    Organizer = null;
                }
                OnPropertyChanged(nameof(Organizer));
            }
        }
    }

    public Organizer Organizer
    {
        get => _organizer;
        set
        {
            _organizer = value;
            OrganizerId = value?.Id;
            OnPropertyChanged(nameof(Organizer));
        }
    }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Date: {Date:yyyy-MM-dd}, Description: {Description}, OrganizerId: {OrganizerId}";
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
