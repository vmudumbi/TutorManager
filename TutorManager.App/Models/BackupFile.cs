using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutorManager.App.Models
{
    public class BackupFile
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SizeFormatted { get; set; }
    }
}
