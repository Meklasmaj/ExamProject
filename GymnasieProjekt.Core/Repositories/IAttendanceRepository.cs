using GymnasieProjekt.Core.Models;

namespace GymnasieProjekt.Core.Repositories;

public interface IAttendanceRepository : IReadRepository<Attendance>
{
    // All attendance registrations for one student, with absence type included
    List<Attendance> GetByStudent(int studentId);
}
