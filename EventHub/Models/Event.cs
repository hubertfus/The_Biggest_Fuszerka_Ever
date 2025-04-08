using EventHub;

using System.ComponentModel;

public class Event : INotifyPropertyChanged
{
    public int Id { get; set; }
    private string _name;
    public string Name 
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
    public DateTime Date { get; set; }
    public string Description { get; set; }
    
    public string ShortDescription => 
        Description != null && Description.Length > 200 
            ? Description.Substring(0, 200) + "..." 
            : Description;
    public string ImageUrl { get; set; }

    public int? OrganizerId { get; set; }

    public Organizer Organizer { get; set; }
    public ICollection<Ticket> Tickets { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
