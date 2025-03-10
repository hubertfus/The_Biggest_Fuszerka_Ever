using System.Collections.ObjectModel;
using Npgsql; // Zmieniono z SqlClient na Npgsql
using DotNetEnv;

namespace EventHub
{
    public class OrganizerManager
    {
        private static OrganizerManager _instance;
        public static OrganizerManager Instance => _instance ??= new OrganizerManager();

        public ObservableCollection<Organizer> Organizers { get; private set; }

        private DatabaseHelper _databaseHelper;

        private OrganizerManager()
        {
            Env.Load();
            _databaseHelper = new DatabaseHelper(Environment.GetEnvironmentVariable("DATABASE_URL"));
            Organizers = new ObservableCollection<Organizer>();
            LoadOrganizers();
        }

        private void LoadOrganizers()
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();
                var query = "SELECT Id, Name, Description, Email, LogoUrl FROM Organizers";
                using (var command = new NpgsqlCommand(query, connection)) 
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var organizer = new Organizer
                        {
                            Id = reader.GetInt32(0),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString(),
                            Email = reader["Email"].ToString(),
                            LogoUrl = reader["LogoUrl"].ToString()
                        };
                        Organizers.Add(organizer);
                    }
                }
            }
        }

        public void AddOrganizer(Organizer newOrganizer)
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();
                var query = "INSERT INTO Organizers (Name, Description, Email, LogoUrl) " +
                            "VALUES (@Name, @Description, @Email, @LogoUrl) RETURNING Id";
                using (var command = new NpgsqlCommand(query, connection)) 
                {
                    command.Parameters.AddWithValue("@Name", newOrganizer.Name);
                    command.Parameters.AddWithValue("@Description", newOrganizer.Description);
                    command.Parameters.AddWithValue("@Email", newOrganizer.Email);
                    command.Parameters.AddWithValue("@LogoUrl", newOrganizer.LogoUrl);
                    
                    newOrganizer.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            Organizers.Add(newOrganizer);
        }

        public void RemoveOrganizer(Organizer organizerToRemove)
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM Organizers WHERE Id = @Id";
                using (var command = new NpgsqlCommand(query, connection)) 
                {
                    command.Parameters.AddWithValue("@Id", organizerToRemove.Id);
                    command.ExecuteNonQuery();
                }
            }
            Organizers.Remove(organizerToRemove);
        }
        
        public void UpdateOrganizer(Organizer updatedOrganizer)
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();
                var query = "UPDATE Organizers SET Name = @Name, Email = @Email, Description = @Description, Logourl = @Logourl " +
                            "WHERE Id = @Id";
                Console.WriteLine($"Updating Organizer ID: {updatedOrganizer.Id}, Name: {updatedOrganizer.Name}, Date: {updatedOrganizer.Description}");

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", updatedOrganizer.Name);
                    command.Parameters.AddWithValue("@Email", updatedOrganizer.Email);
                    command.Parameters.AddWithValue("@Description", updatedOrganizer.Description);
                    command.Parameters.AddWithValue("@Logourl", updatedOrganizer.LogoUrl);
                    command.Parameters.AddWithValue("@Id", updatedOrganizer.Id);
                    command.ExecuteNonQuery();
                }
            }

            var existingEvent = Organizers.FirstOrDefault(e => e.Id == updatedOrganizer.Id);
            if (existingEvent != null)
            {
                existingEvent.Name = updatedOrganizer.Name;
                existingEvent.Email = updatedOrganizer.Email;
                existingEvent.Description = updatedOrganizer.Description;
                existingEvent.LogoUrl = updatedOrganizer.LogoUrl;
            }
        }
    }
}
