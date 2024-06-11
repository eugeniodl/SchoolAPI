using SharedModels;

namespace School_API.Repository.IRepository
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<Attendance> UpdateAsync(Attendance entity);
    }
}
