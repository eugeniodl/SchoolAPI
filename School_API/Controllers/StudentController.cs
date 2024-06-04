using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School_API.Data;
using School_API.Dto;

namespace School_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly SchoolContext _context;
        private readonly ILogger<StudentController> _logger;
        private readonly IMapper _mapper;

        public StudentController(SchoolContext context, 
            ILogger<StudentController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            try
            {
                _logger.LogInformation("Obteniendo los estudiantes");

                var students = await _context.Students.ToListAsync();

                return Ok(_mapper.Map<IEnumerable<StudentDto>>(students));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener los estudiantes: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener los estudiantes");
            }
        }


    }
}
