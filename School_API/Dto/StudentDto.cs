using System.ComponentModel.DataAnnotations;

namespace School_API.Dto
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string? Name { get; set; }
        [Required]
        public bool Registered { get; set; }
    }
}
