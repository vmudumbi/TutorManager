using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using TutorManager.App.Models;

namespace TutorManager.App.Data
{
    public class AttendanceRepository
    {
        public void Add(Attendance a)
        {
            using var con = Db.GetConnection();
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText =
            @"
INSERT INTO Attendance (StudentId, ClassDate, TimeSlot, IsPresent, Hours)
VALUES ($sid, $date, $slot, $present, $hours)
";

            cmd.Parameters.AddWithValue("$sid", a.StudentId);
            cmd.Parameters.AddWithValue("$date", a.ClassDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$slot", a.BatchTime);
            cmd.Parameters.AddWithValue("$present", a.IsPresent ? 1 : 0);
            cmd.Parameters.AddWithValue("$hours", a.HoursWorked);

            cmd.ExecuteNonQuery();
        }

        public void AddBulk(List<Attendance> list)
        {
            using var con = Db.GetConnection();
            con.Open();

            foreach (var a in list)
            {
                var cmd = con.CreateCommand();
                cmd.CommandText =
                @"
        INSERT INTO Attendance (StudentId, ClassDate, Hours)
        VALUES ($sid, $date, $hrs)
        ";

                cmd.Parameters.AddWithValue("$sid", a.StudentId);
                cmd.Parameters.AddWithValue("$date", a.ClassDate);
                cmd.Parameters.AddWithValue("$hrs", a.HoursWorked);

                cmd.ExecuteNonQuery();
            }
        }

        public List<Attendance> GetAll()
        {
            var list = new List<Attendance>();

            using var con = Db.GetConnection();
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Attendance";

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var att = new Attendance();

                att.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                att.StudentId = reader["StudentId"] != DBNull.Value ? Convert.ToInt32(reader["StudentId"]) : 0;

                att.ClassDate = reader["ClassDate"] != DBNull.Value
                    ? DateTime.Parse(reader["ClassDate"].ToString())
                    : DateTime.MinValue;

                att.BatchTime = reader["BatchTime"]?.ToString() ?? "";

                att.IsPresent = reader["IsPresent"] != DBNull.Value &&
                                Convert.ToBoolean(reader["IsPresent"]);

                att.HoursWorked = reader["HoursWorked"] != DBNull.Value
                    ? Convert.ToDecimal(reader["HoursWorked"])
                    : 0;

                list.Add(att);
            }

            return list;
        }

        public List<Attendance> GetByDate(string grade, DateTime date)
        {
            var list = new List<Attendance>();

            using var con = Db.GetConnection();
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText =
            @"
SELECT 
    s.Id AS StudentId,
    a.ClassDate,
    a.HoursWorked,
    a.BatchTime,
    a.IsPresent
FROM Students s
LEFT JOIN Attendance a 
    ON s.Id = a.StudentId
   AND date(a.ClassDate) = date($date)
WHERE s.Grade = $grade
  AND s.IsActive = 1;
";

            cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$grade", grade);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Attendance
                {
                    StudentId = reader.GetInt32(0),
                    ClassDate = reader.IsDBNull(1) ? date : DateTime.Parse(reader.GetString(1)),
                    HoursWorked = reader.IsDBNull(2) ? 0 : Convert.ToDecimal(reader[2]),
                    BatchTime = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    IsPresent = !reader.IsDBNull(4) && reader.GetInt32(4) == 1
                });
            }

            return list;
        }

        public void SaveAttendance(Attendance a)
        {
            using var con = Db.GetConnection();
            con.Open();

            // 1. CHECK IF RECORD EXISTS
            var check = con.CreateCommand();
            check.CommandText =
            @"
    SELECT COUNT(1)
    FROM Attendance
    WHERE StudentId = $sid
      AND date(ClassDate) = date($date);
    ";

            check.Parameters.AddWithValue("$sid", a.StudentId);
            check.Parameters.AddWithValue("$date", a.ClassDate.ToString("yyyy-MM-dd"));

            long exists = (long)check.ExecuteScalar();

            if (exists > 0)
            {
                // 2. UPDATE
                var update = con.CreateCommand();
                update.CommandText =
                @"
        UPDATE Attendance
        SET IsPresent = $present,
            BatchTime = $time,
            HoursWorked = $hours
        WHERE StudentId = $sid
          AND date(ClassDate) = date($date);
        ";

                update.Parameters.AddWithValue("$sid", a.StudentId);
                update.Parameters.AddWithValue("$date", a.ClassDate.ToString("yyyy-MM-dd"));
                update.Parameters.AddWithValue("$time", a.BatchTime);
                update.Parameters.AddWithValue("$present", a.IsPresent ? 1 : 0);
                update.Parameters.AddWithValue("$hours", a.HoursWorked);

                update.ExecuteNonQuery();
            }
            else
            {
                // 3. INSERT
                var insert = con.CreateCommand();
                insert.CommandText =
                @"
        INSERT INTO Attendance 
        (StudentId, ClassDate, BatchTime, IsPresent, HoursWorked)
        VALUES ($sid, $date, $time, $present, $hours);
        ";

                insert.Parameters.AddWithValue("$sid", a.StudentId);
                insert.Parameters.AddWithValue("$date", a.ClassDate.ToString("yyyy-MM-dd"));
                insert.Parameters.AddWithValue("$time", a.BatchTime);
                insert.Parameters.AddWithValue("$present", a.IsPresent ? 1 : 0);
                insert.Parameters.AddWithValue("$hours", a.HoursWorked);

                insert.ExecuteNonQuery();
            }
        }
    }
}