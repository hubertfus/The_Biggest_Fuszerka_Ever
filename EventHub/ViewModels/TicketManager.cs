using System.Collections.ObjectModel;
using Npgsql;
using DotNetEnv;

namespace EventHub
{
    public class TicketManager
    {
        private static TicketManager _instance;
        public static TicketManager Instance => _instance ??= new TicketManager();

        public ObservableCollection<Ticket> Tickets { get; private set; }

        private DatabaseHelper _databaseHelper;

        private TicketManager()
        {
            Env.Load();
            _databaseHelper = new DatabaseHelper(Environment.GetEnvironmentVariable("DATABASE_URL"));
            Tickets = new ObservableCollection<Ticket>();
            LoadTickets();
        }

        private void LoadTickets()
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();
                var query = "SELECT t.id, t.eventname, p.id, p.name, p.email, p.persontype " +
                            "FROM tickets t " +
                            "INNER JOIN people p ON t.ticketholderid = p.id";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int ticketId = reader.GetInt32(0);
                        string eventName = reader.GetString(1);
                        int personId = reader.GetInt32(2);
                        string name = reader.GetString(3);
                        string email = reader.GetString(4);
                        string personType = reader.GetString(5);

                        Person person = personType switch
                        {
                            "VIP" => new VipPerson(personId, name, email),
                            "Disabled" => new DisabledPerson(personId, name, email),
                            _ => new Person(personId, name, email)
                        };

                        var ticket = new Ticket(eventName, person) { Id = ticketId };
                        Tickets.Add(ticket);
                    }
                }
            }
        }

        public void AddTicket(Ticket ticket)
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();

                int personId;
                var getIdQuery = "SELECT id FROM people WHERE name = @Name AND email = @Email";
                using (var command = new NpgsqlCommand(getIdQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", ticket.TicketHolder.Name);
                    command.Parameters.AddWithValue("@Email", ticket.TicketHolder.Email);

                    var result = command.ExecuteScalar();
                    if (result == null)
                    {
                        var insertPersonQuery = "INSERT INTO people (name, email, persontype) VALUES (@Name, @Email, @PersonType) RETURNING id";
                        using (var insertCommand = new NpgsqlCommand(insertPersonQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@Name", ticket.TicketHolder.Name);
                            insertCommand.Parameters.AddWithValue("@Email", ticket.TicketHolder.Email);
                            insertCommand.Parameters.AddWithValue("@PersonType", ticket.TicketHolder.PersonType);
                            personId = (int)insertCommand.ExecuteScalar();
                        }
                    }
                    else
                    {
                        personId = (int)result;
                    }
                }

                var insertTicketQuery = "INSERT INTO tickets (eventname, ticketholderid) VALUES (@EventName, @TicketHolderId) RETURNING id";
                using (var command = new NpgsqlCommand(insertTicketQuery, connection))
                {
                    command.Parameters.AddWithValue("@EventName", ticket.EventName);
                    command.Parameters.AddWithValue("@TicketHolderId", personId);
                    ticket.Id = (int)command.ExecuteScalar();
                }
            }

            Tickets.Add(ticket);
        }

        public void RemoveTicket(Ticket ticket)
        {
            using (var connection = _databaseHelper.GetConnection())
            {
                connection.Open();

                var deleteQuery = "DELETE FROM tickets WHERE id = @TicketId";
                using (var command = new NpgsqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@TicketId", ticket.Id);
                    command.ExecuteNonQuery();
                }
            }

            Tickets.Remove(ticket);
        }
    }
}
