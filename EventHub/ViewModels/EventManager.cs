using System.Collections.ObjectModel;
using Npgsql; // Zmieniono z SqlClient na Npgsql
using DotNetEnv;

namespace EventHub
{
    public class EventManager
    {
        private static EventManager _instance;
        public static EventManager Instance => _instance ??= new EventManager();

        public ObservableCollection<Event> Events { get; private set; }

        private DatabaseHelper _databaseHelper;

        private EventManager()
        {
            Env.Load();
            _databaseHelper = new DatabaseHelper(Environment.GetEnvironmentVariable("DATABASE_URL"));
            Events = new ObservableCollection<Event>();
            LoadEvents();
        }

        private void LoadEvents()
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();
                var query = "SELECT Id, Name, Date, Description, ImageUrl FROM Events";
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ev = new Event
                        {
                            Id = reader.GetInt32(0),
                            Name = reader["Name"].ToString(),
                            Date = reader["Date"].ToString(),
                            Description = reader["Description"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString()
                        };
                        Console.WriteLine($"Loaded event: ID={ev.Id}, Name={ev.Name}");

                        Events.Add(ev);
                    }
                }
            }
        }

        public void AddEvent(Event newEvent)
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();
                var query = "INSERT INTO Events (Name, Date, Description, ImageUrl) " +
                            "VALUES (@Name, @Date, @Description, @ImageUrl) RETURNING Id";

                using (var command = new NpgsqlCommand(query, connection)) 
                {
                    command.Parameters.AddWithValue("@Name", newEvent.Name);
                    command.Parameters.AddWithValue("@Date", newEvent.Date);
                    command.Parameters.AddWithValue("@Description", newEvent.Description);
                    command.Parameters.AddWithValue("@ImageUrl", newEvent.ImageUrl);
    
                    newEvent.Id = Convert.ToInt32(command.ExecuteScalar()); 
                }

            }
            Events.Add(newEvent);
        }
        
        public void UpdateEvent(Event updatedEvent)
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();
                var query = "UPDATE Events SET Name = @Name, Date = @Date, Description = @Description, ImageUrl = @ImageUrl " +
                            "WHERE Id = @Id";
                Console.WriteLine($"Updating event ID: {updatedEvent.Id}, Name: {updatedEvent.Name}, Date: {updatedEvent.Date}");

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", updatedEvent.Name);
                    command.Parameters.AddWithValue("@Date", updatedEvent.Date);
                    command.Parameters.AddWithValue("@Description", updatedEvent.Description);
                    command.Parameters.AddWithValue("@ImageUrl", updatedEvent.ImageUrl);
                    command.Parameters.AddWithValue("@Id", updatedEvent.Id);
                    command.ExecuteNonQuery();
                }
            }

            var existingEvent = Events.FirstOrDefault(e => e.Id == updatedEvent.Id);
            if (existingEvent != null)
            {
                existingEvent.Name = updatedEvent.Name;
                existingEvent.Date = updatedEvent.Date;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.ImageUrl = updatedEvent.ImageUrl;
            }
        }


        public void RemoveEvent(Event eventToRemove)
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM Events WHERE Id = @Id";
                using (var command = new NpgsqlCommand(query, connection)) 
                {
                    command.Parameters.AddWithValue("@Id", eventToRemove.Id);
                    command.ExecuteNonQuery();
                }
            }
            Events.Remove(eventToRemove);
        }
    }
}
