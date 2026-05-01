using System;
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
            (Name, SchoolName, Grade, MathsLevelId, Email, Phone, HourlyRate, Description, IsActive)
            VALUES ($name, $school, $grade, $levelId, $email, $phone, $rate, $desc, $active)
            ";

            cmd.Parameters.AddWithValue("$name", s.Name);
            cmd.Parameters.AddWithValue("$school", s.SchoolName ?? "");
            cmd.Parameters.AddWithValue("$grade", s.Grade ?? "");
            cmd.Parameters.AddWithValue("$levelId", s.LevelId); 
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
            SET Name=$name,
                SchoolName=$school,
                Grade=$grade,
                MathsLevelId=$mathsId, 
                Email=$email,
                Phone=$phone,
                HourlyRate=$rate,
                Description=$desc,
                IsActive=$active
            WHERE Id=$id
            ";

            cmd.Parameters.AddWithValue("$id", s.Id);
            cmd.Parameters.AddWithValue("$name", s.Name);
            cmd.Parameters.AddWithValue("$school", s.SchoolName ?? "");
            cmd.Parameters.AddWithValue("$grade", s.Grade ?? "");
            cmd.Parameters.AddWithValue("$mathsId", s.LevelId); 
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

            // JOIN added to get the LevelName for the UI
            string sql = @"
                SELECT s.*, m.LevelName 
                FROM Students s
                INNER JOIN MathsLevels m ON s.MathsLevelId = m.Id";

            if (onlyActive)
                sql += " WHERE s.IsActive = 1";

            cmd.CommandText = sql;
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Student
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    SchoolName = reader.IsDBNull(reader.GetOrdinal("SchoolName")) ? "" : reader.GetString(reader.GetOrdinal("SchoolName")),
                    Grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? "" : reader.GetString(reader.GetOrdinal("Grade")),
                    LevelName = reader.GetString(reader.GetOrdinal("LevelName")),
                    LevelId = reader.GetInt32(reader.GetOrdinal("MathsLevelId")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
                    Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString(reader.GetOrdinal("Phone")),
                    HourlyRate = reader.GetDecimal(reader.GetOrdinal("HourlyRate")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                    IsActive = reader.GetInt32(reader.GetOrdinal("IsActive"))
                });
            }

            return list;
        }

        public List<MathsLevel> GetLevels()
        {
            var list = new List<MathsLevel>();
            using var con = Db.GetConnection();
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, LevelName FROM MathsLevels";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new MathsLevel { Id = reader.GetInt32(0), LevelName = reader.GetString(1) });
            }
            return list;
        }

        public void Deactivate(int id)
        {
            using var con = Db.GetConnection();
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Students SET IsActive = 0 WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }


    }
}