using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School_API.Data;
using School_API.Dto;
using SharedModels;

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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"ID de estudiante no válido: {id}");
                return BadRequest("ID de estudiante no válido");
            }

            try
            {
                _logger.LogInformation($"Obteniendo estudiante con ID: {id}");

                var student = await _context.Students.FindAsync(id);

                if (student == null)
                {
                    _logger.LogWarning($"No se encontró ningún estudiante con ID: {id}");
                    return NotFound("Estudiante no encontrado.");
                }

                return Ok(_mapper.Map<StudentDto>(student));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener estudiante con ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al obtener el estudiante.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDto>> PostStudent(StudentCreateDto createDto)
        {
            if (createDto == null)
            {
                _logger.LogError("Se recibió un estudiante nulo en la solicitud.");
                return BadRequest("El estudiante no puede ser nulo.");
            }

            try
            {
                _logger.LogInformation($"Creando un nuevo estudiante con nombre: {createDto.Name}");

                // Verificar si el estudiante ya existe
                var existingStudent = await _context.Students
                    .FirstOrDefaultAsync(s => s.Name != null && s.Name.ToLower()
                    == createDto.Name.ToLower());

                if (existingStudent != null)
                {
                    _logger.LogWarning($"El estudiante con nombre '{createDto.Name}' ya existe.");
                    ModelState.AddModelError("NombreExiste", "¡El estudiante con ese nombre ya existe!");
                    return BadRequest(ModelState);
                }

                // Verificar la validez del modelo
                if (!ModelState.IsValid)
                {
                    _logger.LogError("El modelo de estudiante no es válido.");
                    return BadRequest(ModelState);
                }

                // Crear el nuevo estudiante
                var newStudent = _mapper.Map<Student>(createDto);

                _context.Students.Add(newStudent);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Nuevo estudiante '{createDto.Name}' creado con ID: " +
                    $"{newStudent.StudentId}");
                return CreatedAtAction(nameof(GetStudent), new { id = newStudent.StudentId }, newStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear un nuevo estudiante: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al crear un nuevo estudiante.");
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutStudent(int id, StudentUpdateDto updateDto)
        {
            if(updateDto == null || id != updateDto.StudentId)
            {
                return BadRequest("Los datos de entrada no son válidos " +
                    "o el ID del estudiante no coincide.");
            }

            try
            {
                _logger.LogInformation($"Actualizando estudiante con ID: {id}");

                var existingStudent = await _context.Students.FindAsync(id);
                if (existingStudent == null)
                {
                    _logger.LogWarning($"No se encontró ningún estudiante con ID: {id}");
                    return NotFound("El estudiante no existe.");
                }

                // Actualizar solo las propiedades necesarias del estudiante existente
                _mapper.Map(updateDto, existingStudent);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Estudiante con ID {id} actualizado correctamente.");
                return NoContent();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                if(!StudentExists(id))
                {
                    _logger.LogWarning($"No se encontró ningún estudiante con ID: {id}");
                    return NotFound("El estudiante no se encontró durante la actualización.");
                }
                else
                {
                    _logger.LogError($"Error de concurrencia al actualizar el estudiante " +
                        $"con ID: {id}. Detalles: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error interno del servidor al actualizar el estudiante.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el estudiante con ID {id}: " +
                    $"{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al actualizar el estudiante.");
            }
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(s => s.StudentId == id);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                _logger.LogInformation($"Eliminando estudiante con ID: {id}");

                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    _logger.LogWarning($"No se encontró ningún estudiante con ID: {id}");
                    return NotFound("Estudiante no encontrado.");
                }

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Estudiante con ID {id} eliminado correctamente.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el estudiante con ID {id}: " +
                    $"{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Se produjo un error al eliminar el estudiante.");
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchStudent(int id, 
            JsonPatchDocument<StudentUpdateDto> patchDto)
        {
            if(id <= 0)
            {
                return BadRequest("ID de estudiante no válido.");
            }

            try
            {
                _logger.LogInformation($"Aplicando el parche al estudiante con ID: {id}");

                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    _logger.LogWarning($"No se encontró ningún estudiante con ID: {id}");
                    return NotFound("El estudiante no se encontró");
                }

                var studentDto = _mapper.Map<StudentUpdateDto>(student);

                patchDto.ApplyTo(studentDto, ModelState);

                if (!ModelState.IsValid)
                {
                    _logger.LogError("El modelo de estudiante después de aplicar el parche " +
                        "no es válido");
                    return BadRequest(ModelState);
                }

                _mapper.Map(studentDto, student); // Aplicar cambios al objeto original

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        _logger.LogInformation($"Parche aplicado correctamente al estudiante " +
                            $"con ID: {id}");
                        return NoContent();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!StudentExists(id))
                        {
                            _logger.LogWarning($"No se encontró ningún estudiante con ID: {id}");
                            return NotFound();
                        }
                        else
                        {
                            _logger.LogError($"Error de concurrencia al aplicar el parche al estudiante " +
                                $"con ID: {id}. Detalles: {ex.Message}");
                            return StatusCode(StatusCodes.Status500InternalServerError,
                                "Error interno del servidor al aplicar el parche al estudiante.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error al aplicar el parche al estudiante con ID {id}:" +
                            $"{ex.Message}");
                        transaction.Rollback();
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            "Error interno del servidor al aplicar el parche al estudiante.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al aplicar el parche al estudiante con ID {id}:" +
                            $"{ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error interno del servidor al aplicar el parche al estudiante.");

            }
        }
    }
}
