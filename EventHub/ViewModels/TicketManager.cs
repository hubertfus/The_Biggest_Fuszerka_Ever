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
                var query = "SELECT t.id AS ticket_id, e.id AS event_id, e.name AS event_name, " +
                            "p.id AS person_id, p.name AS person_name, p.email, p.persontype " +
                            "FROM tickets t " +
                            "INNER JOIN people p ON t.ticketholderid = p.id " +
                            "INNER JOIN events e ON t.eventid = e.id";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int ticketId = reader.GetInt32(0);  // t.id
                        int eventId = reader.GetInt32(1);   // e.id
                        string eventName = reader.GetString(2);  // e.name
                        int personId = reader.GetInt32(3);  // p.id
                        string personName = reader.GetString(4);  // p.name
                        string email = reader.GetString(5);  // p.email
                        string personType = reader.GetString(6);  // p.persontype

                        Person person = personType switch
                        {
                            "VIP" => new VipPerson(personId, personName, email),
                            "Disabled" => new DisabledPerson(personId, personName, email),
                            _ => new Person(personId, personName, email)
                        };

                        var ticket = new Ticket(EventManager.Instance.Events.First(e=> e.Id == eventId), person) { Id = ticketId };
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
                            ticket.TicketHolder.Id = personId;
                            Console.WriteLine(ticket.TicketHolder);
                        }
                    }
                    else
                    {
                        personId = (int)result;
                        ticket.TicketHolder.Id = personId;
                        Console.WriteLine(ticket.TicketHolder);
                    }
                }

                var insertTicketQuery = "INSERT INTO tickets (eventid, ticketholderid) VALUES (@EventId, @TicketHolderId) RETURNING id";
                using (var command = new NpgsqlCommand(insertTicketQuery, connection))
                {
                    command.Parameters.AddWithValue("@EventId", ticket.Event.Id); 
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

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var deleteTicketQuery = "DELETE FROM tickets WHERE id = @TicketId";
                        using (var command = new NpgsqlCommand(deleteTicketQuery, connection))
                        {
                            command.Parameters.AddWithValue("@TicketId", ticket.Id);
                            command.ExecuteNonQuery();
                        }

                        var deleteOrphanPeopleQuery = @"
                            DELETE FROM people
                            WHERE id = @TicketholderId
                            AND NOT EXISTS (SELECT 1 FROM tickets WHERE ticketholderid = @TicketholderId)";

                        using (var command = new NpgsqlCommand(deleteOrphanPeopleQuery, connection))
                        {
                            command.Parameters.AddWithValue("@TicketholderId", TicketManager.Instance.Tickets.First(t => t.Id == ticket.Id).TicketHolder.Id);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            Tickets.Remove(ticket);
        }

    }
}
