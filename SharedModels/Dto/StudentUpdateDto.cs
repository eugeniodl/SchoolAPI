﻿using System.ComponentModel.DataAnnotations;

namespace SharedModels.Dto
{
    public class StudentUpdateDto
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
        [Required]
        public bool Registered { get; set; }
    }
}
