using Microsoft.Data.Sqlite;
using System;
using System.Configuration;
using System.IO;
using TutorManager.App.Utility;

namespace TutorManager.App.Data
{
    public static class Db
    {
        // Path logic: Stores the DB in the same folder as the EXE for easier backups
        private static string dbPath = Path.Combine(AppContext.BaseDirectory, "tutor.db");
        private static string connectionString = $"Data Source={dbPath}";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }

        public static void Init()
        {
            bool.TryParse(ConfigurationManager.AppSettings["RebuildDatabase"], out bool shouldRebuild);

            using var con = GetConnection();
            con.Open();

            var cmd = con.CreateCommand();

            string dropScript = "";
            if (shouldRebuild)
            {
                dropScript = @"
                    DROP TABLE IF EXISTS Attendance;
                    DROP TABLE IF EXISTS Students;                    
                    DROP TABLE IF EXISTS Users;
                    DROP TABLE IF EXISTS MathsLevels;";
            }

            cmd.CommandText = dropScript + @"
    CREATE TABLE IF NOT EXISTS MathsLevels (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        LevelName TEXT NOT NULL UNIQUE
    );

    CREATE TABLE IF NOT EXISTS Students (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Name TEXT NOT NULL,
        SchoolName TEXT NOT NULL,
        Grade TEXT NOT NULL,
        MathsLevelId INTEGER NOT NULL,
        Email TEXT,
        Phone TEXT,
        HourlyRate REAL NOT NULL,
        Description TEXT,
        IsActive INTEGER DEFAULT 1,
        FOREIGN KEY (MathsLevelId) REFERENCES MathsLevels(Id)
    );

    CREATE TABLE IF NOT EXISTS Attendance(
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        StudentId INTEGER NOT NULL,
        ClassDate TEXT NOT NULL,
        BatchTime TEXT,
        IsPresent INTEGER DEFAULT 1,
        HoursWorked REAL DEFAULT 1,
        FOREIGN KEY (StudentId) REFERENCES Students(Id)
    );

    CREATE TABLE IF NOT EXISTS Users (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Username TEXT NOT NULL UNIQUE,
        Password TEXT NOT NULL,
        Role TEXT DEFAULT 'Staff',
        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
    );";

            cmd.ExecuteNonQuery();

            // 1. Only seed Admin if rebuilding
            if (shouldRebuild)
            {
                SeedDefaultUser(con);                
            }

            // 2. ALWAYS seed Maths Levels from text file on every startup
            SeedMathsLevels(con);
        }

        private static void SeedMathsLevels(SqliteConnection con)
        {
            try
            {
                // 1. Read the path from your App.config key
                string filePath = ConfigurationManager.AppSettings["MathsFilePath"];

                // 2. Validate the path exists
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine("Maths file not found at: " + filePath);
                    return;
                }

                // 3. Read and clean levels from the text file
                var levels = File.ReadAllLines(filePath)
                                 .Where(l => !string.IsNullOrWhiteSpace(l))
                                 .Select(l => l.Trim());

                foreach (var level in levels)
                {
                    using var cmd = con.CreateCommand();
                    // INSERT OR IGNORE ensures existing IDs don't change
                    cmd.CommandText = "INSERT OR IGNORE INTO MathsLevels (LevelName) VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", level);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Syncing maths.txt failed: " + ex.Message);
            }
        }

        private static void SeedDefaultUser(SqliteConnection con)
        {
            string username = "admin";
            string plainPassword = "admin123";
            UserRepository _userRepo = new UserRepository();
            _userRepo.Add(username, plainPassword, "Admin");
        }
    }
}