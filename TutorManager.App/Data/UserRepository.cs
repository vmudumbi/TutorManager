using System;
using Microsoft.Data.Sqlite;
using TutorManager.App.Models;
using BCrypt.Net; 

namespace TutorManager.App.Data
{
    public class UserRepository
    {
        // Add a new user with a Salted Hash
        public bool Add(string username, string password,string role)
        {
            try
            {
                // Generate a salt and hash the password in one step
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                using var con = Db.GetConnection();
                con.Open();

                var cmd = con.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Users (Username, Password, Role) 
                    VALUES ($user, $pass, $role)";

                cmd.Parameters.AddWithValue("$user", username);
                cmd.Parameters.AddWithValue("$pass", hashedPassword);
                cmd.Parameters.AddWithValue("$role", role);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
        }        
        public bool Validate(string username, string password)
        {
            try
            {
                using var con = Db.GetConnection();
                con.Open();

                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT Password FROM Users WHERE Username = $user";
                cmd.Parameters.AddWithValue("$user", username);

                var storedHash = cmd.ExecuteScalar()?.ToString();

                if (string.IsNullOrEmpty(storedHash)) return false;

                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
            catch
            {
                return false;
            }
        }

        public bool HasUsers()
        {
            using var con = Db.GetConnection();
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM Users";
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}