using System.ComponentModel.DataAnnotations;

namespace SharedModels
{
    public class Student
    {
        public int StudentId { get; set; }
        [StringLength(50)]
        public string? Name { get; set;}
        public bool Registered { get; set; }

        public ICollection<Attendance>? Attendances { get; set; }
    }
}
