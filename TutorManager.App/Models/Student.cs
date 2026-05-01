using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorManager.App.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string SchoolName { get; set; } = "";
        public string Grade { get; set; } = "";
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal HourlyRate { get; set; }
        public string? Description { get; set; }
        public int IsActive { get; set; } = 1;
    }
}
