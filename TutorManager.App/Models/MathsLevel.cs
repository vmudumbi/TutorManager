using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorManager.App.Models
{
    public class MathsLevel
    {
        public int Id { get; set; }
        public string LevelName { get; set; }
        public override string ToString() => LevelName;
    }
}
