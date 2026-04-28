using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using TutorManager.App.Models;

namespace TutorManager.App.Data
{
    public class StudentRepository
    {
        public void Add(Student s)
        {
            using var con = Db.GetConnection();
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText =
            @"
            INSERT INTO Students 
            (Name, SchoolName, Grade, Email, Phone, HourlyRate, Description, IsActive)
            VALUES ($name, $school, $grade, $email, $phone, $rate, $desc, $active)
            ";

            cmd.Parameters.AddWithValue("$name", s.Name);
            cmd.Parameters.AddWithValue("$school", s.SchoolName ?? "");
            cmd.Parameters.AddWithValue("$grade", s.Grade ?? "");
            cmd.Parameters.AddWithValue("$email", s.Email ?? "");
            cmd.Parameters.AddWithValue("$phone", s.Phone ?? "");
            cmd.Parameters.AddWithValue("$rate", s.HourlyRate);
            cmd.Parameters.AddWithValue("$desc", s.Description ?? "");
            cmd.Parameters.AddWithValue("$active", s.IsActive);

            cmd.ExecuteNonQuery();
        }

        public void Update(Student s)
        {
            using var con = Db.GetConnection();
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText =
            @"
            UPDATE Students
            SET SchoolName=$school,
                Grade=$grade,
                Email=$email,
                Phone=$phone,
                HourlyRate=$rate,
                Description=$desc,
                IsActive=$active
            WHERE Id=$id
            ";

            cmd.Parameters.AddWithValue("$id", s.Id);
            cmd.Parameters.AddWithValue("$school", s.SchoolName ?? "");
            cmd.Parameters.AddWithValue("$grade", s.Grade ?? "");
            cmd.Parameters.AddWithValue("$email", s.Email ?? "");
            cmd.Parameters.AddWithValue("$phone", s.Phone ?? "");
            cmd.Parameters.AddWithValue("$rate", s.HourlyRate);
            cmd.Parameters.AddWithValue("$desc", s.Description ?? "");
            cmd.Parameters.AddWithValue("$active", s.IsActive);

            cmd.ExecuteNonQuery();
        }

        public List<Student> GetAll(bool onlyActive = true)
        {
            var list = new List<Student>();

            using var con = Db.GetConnection();
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = onlyActive
                ? "SELECT * FROM Students WHERE IsActive = 1"
                : "SELECT * FROM Students";

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Student
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    SchoolName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Grade = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Phone = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    HourlyRate = reader.IsDBNull(6) ? 0 : Convert.ToDecimal(reader[6]),
                    Description = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    IsActive = reader.GetInt32(8)
                });
            }

            return list;
        }

        public void Deactivate(int id)
        {
            using var con = Db.GetConnection();
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText =
            @"
    UPDATE Students
    SET IsActive = 0
    WHERE Id = $id
    ";

            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }
    }
}   