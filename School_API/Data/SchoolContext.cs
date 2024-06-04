using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace School_API.Data
{
    public class SchoolContext : DbContext 
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) :
            base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
    }
}
