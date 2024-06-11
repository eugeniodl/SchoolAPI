using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Dto
{
    public class AttendanceUpdateDto
    {
        [Required]
        public int AttendanceId { get; set; }
        [Required]
        public int StudentId { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public bool Present { get; set; }
    }
}
