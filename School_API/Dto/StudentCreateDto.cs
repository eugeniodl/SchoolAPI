using System.ComponentModel.DataAnnotations;

namespace School_API.Dto
{
    public class StudentCreateDto
    {
        public string? Name { get; set; }
        [Required]
        public bool Registered { get; set; }
    }
}
