using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School_API.Repository.IRepository;
using SharedModels;
using SharedModels.Dto;

namespace School_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly ILogger<AttendanceController> _logger;
        private readonly IMapper _mapper;
        public AttendanceController(IStudentRepository studentRepo, 
            IAttendanceRepository attendanceRepo,
            ILogger<AttendanceController> logger,
            IMapper mapper)
        {
            _studentRepo = studentRepo;
            _attendanceRepo = attendanceRepo;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAttendances()
        {
            try
            {
                _logger.LogInformation("Obteniendo las asistencias");

                var attendances = await _attendanceRepo.GetAllAsync();

                return Ok(_mapper.Map<IEnumerable<AttendanceDto>>(attendances));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener las asistencias: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener las asistencias.");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AttendanceDto>> GetAttendance(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"ID de asistencia no válido: {id}");
                return BadRequest("ID de asistencia no válido");
            }

            try
            {
                _logger.LogInformation($"Obteniendo asistencia con ID: {id}");

                var attendance = await _attendanceRepo.GetByIdAsync(id);

                if (attendance == null)
                {
                    _logger.LogWarning($"No se encontró ninguna asistencia con ID: {id}");
                    return NotFound("Asistencia no encontrada.");
                }

                return Ok(_mapper.Map<AttendanceDto>(attendance));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener asistencia con ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener la asistencia.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AttendanceDto>> PostAttendance(AttendanceCreateDto createDto)
        {
            if (createDto == null)
            {
                _logger.LogError("Se recibió una asistencia nula en la solicitud.");
                return BadRequest("La asistencia no puede ser nula.");
            }

            try
            {
                _logger.LogInformation($"Creando una nueva para el estudiante con ID: {createDto.StudentId} " +
                    $"en la fecha: {createDto.Date}");

                // Verificar si el estudiante existe
                var studentExists = await _studentRepo.ExistsAsync(s => s.StudentId == createDto.StudentId);
                
                if (!studentExists)
                {
                    _logger.LogWarning($"El estudiante con ID '{createDto.StudentId}' no existe.");
                    ModelState.AddModelError("EstudianteNoExiste", "¡El estudiante con existe!");
                    return BadRequest(ModelState);
                }

                // Verificar si la aistencia ya existe para la fecha y el estudiante
                var existingAttendance = await _attendanceRepo
                    .GetAsync(a => a.StudentId == createDto.StudentId && a.Date == createDto.Date);

                if(existingAttendance != null)
                {
                    _logger.LogWarning($"La asistencia para el estudiante con ID '{createDto.StudentId}' " +
                        $"ya existe para la fecha '{createDto.Date}'");
                    ModelState.AddModelError("AsistenciaExiste", "¡La asistencia para esa fecha ya existe!");
                    return BadRequest(ModelState);
                }

                // Verificar la validez del modelo
                if (!ModelState.IsValid)
                {
                    _logger.LogError("El modelo de asistencia no es válido.");
                    return BadRequest(ModelState);
                }

                // Crear la nueva asistencia
                var newAttendance = _mapper.Map<Attendance>(createDto);

                await _attendanceRepo.CreateAsync(newAttendance);

                _logger.LogInformation($"Nueva asistencia creada con ID: " +
                    $"{newAttendance.AttendanceId}");
                return CreatedAtAction(nameof(GetAttendance), new { id = newAttendance.AttendanceId }, newAttendance);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear una nueva asistencia: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al crear una nueva asistencia.");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAttendance(int id, AttendanceUpdateDto updateDto)
        {
            if(updateDto == null || id != updateDto.AttendanceId)
            {
                return BadRequest("Los datos de entrada no son válidos " +
                    "o el ID de asistencia no coincide.");
            }

            try
            {
                _logger.LogInformation($"Actualizando asistencia con ID: {id}");

                var existingAttendance = await _attendanceRepo.GetByIdAsync(id);
                if (existingAttendance == null)
                {
                    _logger.LogWarning($"No se encontró ninguna asistencia con ID: {id}");
                    return NotFound("La asistencia no existe");
                }

                // Verificar si el estudiante existe
                var studentExists = await _studentRepo.ExistsAsync(s => s.StudentId == updateDto.StudentId);
                if(!studentExists)
                {
                    _logger.LogWarning($"El estudiante con ID '{updateDto.StudentId}' no existe.");
                    ModelState.AddModelError("EstudianteNoExiste", "¡El estudiante no existe!");
                    return BadRequest(ModelState);
                }


                // Actualizar solo las propiedades necesarias de la asistenca existente
                _mapper.Map(updateDto, existingAttendance);

                await _attendanceRepo.SaveChangesAsync();

                _logger.LogInformation($"Asistencia con ID {id} actualizada correctamente.");

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await _attendanceRepo.ExistsAsync(s => s.AttendanceId == id))
                {
                    _logger.LogWarning($"No se encontró ninguna assitenca con ID: {id}");
                    return NotFound("La asistencia no se encontró durante la actualización.");
                }
                else
                {
                    _logger.LogError($"Error de concurrencia al actualizar la asistencia " +
                        $"con ID: {id}. Detalles: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error interno del servidor al actualizar la asistencia.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la asistencia con ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al actualizar la asistencia.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            try
            {
                _logger.LogInformation($"Eliminando asistencia con ID: {id}");

                var attendance = await _attendanceRepo.GetByIdAsync(id);
                if (attendance == null)
                {
                    _logger.LogWarning($"No se encontró ninguna asistencia con ID: {id}");
                    return NotFound("Asistencia no encontrada.");
                }

                await _attendanceRepo.DeleteAsync(attendance);

                _logger.LogInformation($"Asistencia con ID {id} eliminada correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la asistencia con ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Se produjo un error al eliminar la asistencia.");
            }
        }
        
    }
}
