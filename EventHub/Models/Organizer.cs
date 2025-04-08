using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class Organizer : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    
    public string ShortDescription => 
        Description != null && Description.Length > 200 
            ? Description.Substring(0, 200) + "..." 
            : Description;

    public string? Email { get; set; }
    public string? LogoUrl { get; set; }

    public ICollection<Event> Events { get; set; }

    public Organizer()
    {
        Events = new HashSet<Event>();
    }

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"Id: {Id}\n Name: {Name} \n Description: {Description} \n Email: {Email}";
    }
}