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
            string orderFilePath = Path.Combine(folderPath, "seeder_order.txt");

            if (!File.Exists(orderFilePath))
            {
                Console.WriteLine("❌ 'seeder_order.txt' file not found.");
                return;
            }
            
            var seederOrder = File.ReadAllLines(orderFilePath).Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

            if (seederOrder.Count == 0)
            {
                Console.WriteLine("❌ 'seeder_order.txt' file is empty.");
                return;
            }
            
            string[] sqlFiles = Directory.GetFiles(folderPath, "*.sql");
            
            var filesToExecute = seederOrder
                .Where(fileName => sqlFiles.Any(file => Path.GetFileName(file) == fileName))
                .Select(fileName => Path.Combine(folderPath, fileName))
                .ToList();

            if (filesToExecute.Count == 0)
            {
                Console.WriteLine("❌ No SQL files to execute in the specified order.");
                return;
            }

            Console.WriteLine("🔄 SQL files found. Starting execution...");

            var filesToRetry = new Queue<string>(filesToExecute);
            var retryLimit = 3;
            var attempts = new Dictionary<string, int>();

            while (filesToRetry.Count > 0)
            {
                string currentFile = filesToRetry.Dequeue();
                bool seedingSuccessful = false;

                if (attempts.ContainsKey(currentFile) && attempts[currentFile] >= retryLimit)
                {
                    Console.WriteLine($"❌ File {currentFile} was skipped due to exceeding the retry limit.");
                    continue;
                }

                try
                {
                    string sql = File.ReadAllText(currentFile);
                    ExecuteSql(sql, currentFile);
                    Console.WriteLine($"✅ Executed: {Path.GetFileName(currentFile)}");

                    seedingSuccessful = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error in file {Path.GetFileName(currentFile)}: {ex.Message}");
                }

                if (!seedingSuccessful)
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

            Console.WriteLine("🎉 All seeders have been executed or skipped.");
        }
        else
        {
            Console.WriteLine("❌ The provided path does not exist.");
        }
    }

    static void ExecuteSql(string sql, string fileName)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            
            ExecuteSqlCommand(connection, sql);
        }
    }

    static void ExecuteSqlCommand(NpgsqlConnection connection, string sql)
    {
        using (var command = new NpgsqlCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }
}
