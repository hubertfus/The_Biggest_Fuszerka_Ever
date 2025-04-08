namespace EventHub;

public class DatabaseConnectionSettings
{
    public string ServerName { get; set; }
    public string DatabaseName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public string GetConnectionString()
    {
        return $"Host={ServerName};Database={DatabaseName};Username={Username};Password={Password}";
    }
}