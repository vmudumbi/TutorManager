using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Configuration;
using System.IO;
using TutorManager.App.Utility;

namespace TutorManager.App.Data
{
    public static class Db
    {
        private static string executableFolder = AppContext.BaseDirectory;

        private static string projectRoot =
            Path.GetFullPath(Path.Combine(executableFolder, @"..\..\..\"));

        private static string dbFolder =
            Path.Combine(projectRoot, "Data");

        private static string dbPath =
            Path.Combine(dbFolder, "tutor.db");

        private static string connectionString =
            $"Data Source={dbPath}";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }

        public static void Init()
        {
            if (!Directory.Exists(dbFolder))
                Directory.CreateDirectory(dbFolder);

            bool.TryParse(ConfigurationManager.AppSettings["RebuildDatabase"], out bool shouldRebuild);

            using var con = GetConnection();
            con.Open();

            var cmd = con.CreateCommand();

            string dropScript = "";
            if (shouldRebuild)
            {
                dropScript = @"
                    DROP TABLE IF EXISTS Students;
                    DROP TABLE IF EXISTS Attendance;
                    DROP TABLE IF EXISTS Users;";
            }

            cmd.CommandText = dropScript +
            @"
    CREATE TABLE IF NOT EXISTS Students (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Name TEXT NOT NULL,
        SchoolName TEXT NOT NULL,
        Grade TEXT NOT NULL,
        Email TEXT,
        Phone TEXT,
        HourlyRate REAL NOT NULL,
        Description TEXT,
        IsActive INTEGER DEFAULT 1
    );

    CREATE TABLE IF NOT EXISTS Attendance(
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        StudentId INTEGER NOT NULL,
        ClassDate TEXT NOT NULL,
        BatchTime TEXT,
        IsPresent INTEGER DEFAULT 1,
        HoursWorked REAL DEFAULT 1
    );

     CREATE TABLE IF NOT EXISTS Users (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Username TEXT NOT NULL UNIQUE,
        Password TEXT NOT NULL,
        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
    );

    CREATE UNIQUE INDEX IF NOT EXISTS idx_attendance_unique
    ON Attendance(StudentId, ClassDate, BatchTime);
    ";

            cmd.ExecuteNonQuery();

            if (shouldRebuild)
            {
                SeedDefaultUser(con);
            }
        }

        private static void SeedDefaultUser(SqliteConnection con)
        {
            string username = "admin";
            string plainPassword = "admin123";
            UserRepository _userRepo = new UserRepository();
            _userRepo.Add(username, plainPassword);
        }
    }
}