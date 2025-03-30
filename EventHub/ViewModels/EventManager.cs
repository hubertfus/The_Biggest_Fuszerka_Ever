using System;
using System.Collections.ObjectModel;
using System.Linq;
using Npgsql;
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
                var query = "SELECT Id, Name, Date, Description, ImageUrl, OrganizerId FROM Events";
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ev = new Event
                        {
                            Id = reader.GetInt32(0),
                            Name = reader["Name"].ToString(),
                            Date = reader.GetDateTime(2),
                            Description = reader["Description"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString(),
                            OrganizerId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                        };

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
                var query = "INSERT INTO Events (Name, Date, Description, ImageUrl, OrganizerId) " +
                            "VALUES (@Name, @Date, @Description, @ImageUrl, @OrganizerId) RETURNING Id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", newEvent.Name);
                    command.Parameters.AddWithValue("@Date", newEvent.Date); 
                    command.Parameters.AddWithValue("@Description", newEvent.Description);
                    command.Parameters.AddWithValue("@ImageUrl", newEvent.ImageUrl);
                    command.Parameters.AddWithValue("@OrganizerId", newEvent.OrganizerId);

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
                var query = "UPDATE Events SET Name = @Name, Date = @Date, Description = @Description, " +
                            "ImageUrl = @ImageUrl, OrganizerId = @OrganizerId WHERE Id = @Id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", updatedEvent.Name);
                    command.Parameters.AddWithValue("@Date", updatedEvent.Date);
                    command.Parameters.AddWithValue("@Description", updatedEvent.Description);
                    command.Parameters.AddWithValue("@ImageUrl", updatedEvent.ImageUrl);
                    command.Parameters.AddWithValue("@OrganizerId", updatedEvent.OrganizerId);
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
                existingEvent.Organizer = updatedEvent.Organizer;
            }
        }

        public void RemoveEvent(Event eventToRemove)
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();

                var deleteTicketsQuery = "DELETE FROM Tickets WHERE EventId = @EventId";
                using (var deleteTicketsCommand = new NpgsqlCommand(deleteTicketsQuery, connection))
                {
                    deleteTicketsCommand.Parameters.AddWithValue("@EventId", eventToRemove.Id);
                    deleteTicketsCommand.ExecuteNonQuery();
                }


                var ticketsToRemove = new List<Ticket>();
                foreach (Ticket ticket in TicketManager.Instance.Tickets)
                {
                    if (ticket.Event.Id == eventToRemove.Id)
                    {
                        ticketsToRemove.Add(ticket);
                    }
                }
                
                foreach (var ticket in ticketsToRemove)
                {
                    TicketManager.Instance.Tickets.Remove(ticket);
                }

                var deleteEventQuery = "DELETE FROM Events WHERE Id = @Id";
                using (var deleteEventCommand = new NpgsqlCommand(deleteEventQuery, connection))
                {
                    deleteEventCommand.Parameters.AddWithValue("@Id", eventToRemove.Id);
                    deleteEventCommand.ExecuteNonQuery();
                }
            }
            Events.Remove(eventToRemove);
        }


    }
}
