using Microsoft.EntityFrameworkCore;
using School_API.Data;
using School_API.Repository.IRepository;
using SharedModels;

namespace School_API.Repository
{
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        private readonly SchoolContext _context;

        public AttendanceRepository(SchoolContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Attendance> UpdateAsAsync(Attendance entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
