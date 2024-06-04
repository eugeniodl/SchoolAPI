using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int StudentId { get; set; }
        public DateOnly Date { get; set; }
        public bool Present { get; set; }
        public Student? Student { get; set; }
    }
}
