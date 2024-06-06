using SharedModels;

namespace School_API.Repository.IRepository
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student> UpdateAsync(Student entity);
    }
}
