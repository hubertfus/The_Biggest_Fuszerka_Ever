using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Npgsql;

class Program
{
    static string connectionString = "";

    static void Main()
    {
        Console.Write("Enter the path to the folder with SQL files: ");
        string folderPath = Console.ReadLine();

        if (Directory.Exists(folderPath))
        {
            string orderFilePath = Path.Combine(folderPath, "migration_order.txt");

            if (!File.Exists(orderFilePath))
            {
                Console.WriteLine("❌ 'migration_order.txt' file not found.");
                return;
            }

            var migrationOrder = File.ReadAllLines(orderFilePath).Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            if (migrationOrder.Count == 0)
            {
                Console.WriteLine("❌ 'migration_order.txt' file is empty.");
                return;
            }

            string[] sqlFiles = Directory.GetFiles(folderPath, "*.sql");

            var filesToExecute = migrationOrder
                .Where(fileName => sqlFiles.Any(file => Path.GetFileName(file) == fileName))
                .Select(fileName => Path.Combine(folderPath, fileName))
                .ToList();

            if (filesToExecute.Count == 0)
            {
                Console.WriteLine("❌ No SQL files to execute according to the specified order.");
                return;
            }

            Console.WriteLine("🔄 SQL files found. Starting execution...");

            var filesToRetry = new Queue<string>(filesToExecute); 
            var retryLimit = 3;
            var attempts = new Dictionary<string, int>();

            while (filesToRetry.Count > 0)
            {
                string currentFile = filesToRetry.Dequeue(); 
                bool migrationSuccessful = false;

                if (attempts.ContainsKey(currentFile) && attempts[currentFile] >= retryLimit)
                {
                    Console.WriteLine($"❌ File {currentFile} was skipped due to exceeding the retry limit.");
                    continue;
                }

                try
                {
                    string sql = File.ReadAllText(currentFile);
                    ExecuteSqlIfTableExists(sql, currentFile);
                    Console.WriteLine($"✅ Executed: {Path.GetFileName(currentFile)}");

                    migrationSuccessful = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error in file {Path.GetFileName(currentFile)}: {ex.Message}");
                }

                if (!migrationSuccessful)
                {
                    if (!attempts.ContainsKey(currentFile))
                    {
                        attempts[currentFile] = 0;
                    }
                    attempts[currentFile]++;
                    filesToRetry.Enqueue(currentFile); 

                    if (attempts[currentFile] < retryLimit)
                    {
                        Console.WriteLine($"⏳ File {currentFile} will try again...");
                    }
                }
            }

            Console.WriteLine("🎉 All migrations have been executed or skipped.");
        }
        else
        {
            Console.WriteLine("❌ The provided path does not exist.");
        }
    }

    static void ExecuteSqlIfTableExists(string sql, string fileName)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            if (sql.Contains("CREATE TABLE IF NOT EXISTS"))
            {
                string tableName = ExtractTableNameFromSql(sql);
                if (!IsTableExist(connection, tableName))
                {
                    ExecuteSql(connection, sql);
                }
                else
                {
                    Console.WriteLine($"Table {tableName} already exists, so skipping file: {fileName}");
                }
            }
            else
            {
                ExecuteSql(connection, sql);
            }
        }
    }

    static void ExecuteSql(NpgsqlConnection connection, string sql)
    {
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    static bool IsTableExist(NpgsqlConnection connection, string tableName)
    {
        string checkTableQuery = $"SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName}');";
        using (var command = new NpgsqlCommand(checkTableQuery, connection))
        {
            var result = command.ExecuteScalar();
            return Convert.ToBoolean(result);
        }
    }

    static string ExtractTableNameFromSql(string sql)
    {
        var parts = sql.Split(' ');
        return parts[2];  
    }
}
