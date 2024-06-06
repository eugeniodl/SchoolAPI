using Microsoft.EntityFrameworkCore;
using School_API.Data;
using School_API.Repository.IRepository;
using SharedModels;

namespace School_API.Repository
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        private readonly SchoolContext _context;

        public StudentRepository(SchoolContext context) : base(context)
        {
            _context = context;
        }
        
        public async Task<Student> UpdateAsync(Student entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
