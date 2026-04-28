using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorManager.App.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime ClassDate { get; set; }
        public string BatchTime { get; set; }
        public bool IsPresent { get; set; }
        public decimal HoursWorked { get; set; }
    }
}
