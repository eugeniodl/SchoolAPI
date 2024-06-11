using System.ComponentModel.DataAnnotations;

namespace SharedModels.Dto
{
    public class StudentCreateDto
    {
        [StringLength(50)]
        public string? Name { get; set; }
        [Required]
        public bool Registered { get; set; }
    }
}
