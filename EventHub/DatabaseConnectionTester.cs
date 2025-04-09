using Npgsql;

namespace EventHub;

public static class DatabaseConnectionTester
{
    public static bool TestConnection(string connectionString, out string message)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                message = "✅ Connection successful.";
                return true;
            }
        }
        catch (Exception ex)
        {
            message = $"❌ Connection failed: {ex.Message}";
            return false;
        }
    }
}