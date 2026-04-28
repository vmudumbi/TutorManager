using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace TutorManager.App.Data
{
    public static class Db
    {
        private static string projectRoot =
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));

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

            using var con = GetConnection();
            con.Open();

            var cmd = con.CreateCommand();

            cmd.CommandText =
            @"
  --DROP TABLE IF EXISTS Students;
  --DROP TABLE IF EXISTS Attendance;

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

    CREATE UNIQUE INDEX IF NOT EXISTS idx_attendance_unique
    ON Attendance(StudentId, ClassDate, BatchTime);
    ";

            cmd.ExecuteNonQuery();
        }
    }
}