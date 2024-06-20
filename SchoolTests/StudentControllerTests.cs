using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using School_API.Controllers;
using School_API.Repository.IRepository;
using SharedModels;
using SharedModels.Dto;

namespace SchoolTests
{
    public class StudentControllerTests
    {
        private readonly Mock<IStudentRepository> _mockRepo;
        private readonly Mock<ILogger<StudentController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly StudentController _controller;

        public StudentControllerTests()
        {
            _mockRepo = new Mock<IStudentRepository>();
            _mockLogger = new Mock<ILogger<StudentController>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new StudentController(_mockRepo.Object,
                _mockLogger.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetStudents_ReturnOkResult_WithListOfStudents()
        {
            // Arrange
            var students = 
                new List<Student> { new Student { StudentId = 1, Name = "Ana Pérez"} };
            var studentDtos =
                new List<StudentDto> { new StudentDto { StudentId = 1, Name = "Ana Pérez" } };

            _mockRepo.Setup(repo => repo.GetAllAsync(null)).ReturnsAsync(students);
            _mockMapper.Setup(mapper => mapper
            .Map<IEnumerable<StudentDto>>(It.IsAny<IEnumerable<Student>>()))
                .Returns(studentDtos);

            // Act
            var result = await _controller.GetStudents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<StudentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("Ana Pérez", returnValue[0].Name);
        }

        [Fact]
        public async Task GetStudent_ReturnNotFound_WhenStudentNotFound()
        {
            // Arrange
            _mockRepo.Setup(repo => 
            repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Student)null);

            // Act
            var result = await _controller.GetStudent(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Estudiante no encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetStudent_ReturnOkResult_WhenStudentFound()
        {
            // Arrange
            var student = new Student { StudentId = 1, Name = "José López" };
            var studentDto = new StudentDto { StudentId = 1, Name = "José López" };

            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(student);
            _mockMapper.Setup(mapper => mapper.Map<StudentDto>(It.IsAny<Student>()))
                .Returns(studentDto);

            // Act
            var result = await _controller.GetStudent(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<StudentDto>(okResult.Value);
            Assert.Equal("José López", returnValue.Name);
        }

        [Fact]
        public async Task PostStudent_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            var studentCreateDto = new StudentCreateDto { Name = "" };

            // Act
            var result = await _controller.PostStudent(studentCreateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteStudent_ReturnsNoContent_WhenStudentIsDeleted()
        {
            // Arrange
            var studentId = 1;
            var existingStudent = new Student { StudentId = studentId, Name = "Juan Dávila" };

            _mockRepo.Setup(repo => repo.GetByIdAsync(studentId)).ReturnsAsync(existingStudent);
            _mockRepo.Setup(repo => repo.DeleteAsync(existingStudent)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteStudent(studentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
