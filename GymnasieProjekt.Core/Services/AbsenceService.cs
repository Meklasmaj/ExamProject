using GymnasieProjekt.Core.Models;
using GymnasieProjekt.Core.Repositories;

namespace GymnasieProjekt.Core.Services;

// Business logic for absence. Kept in Core, not in a ViewModel, so it can be tested and reused.
public class AbsenceService
{
    private readonly IAttendanceRepository _attendanceRepository;

    public AbsenceService(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public AbsenceSummary GetTotalAbsence(int studentId)
    {
        return Calculate(_attendanceRepository.GetByStudent(studentId));
    }

    // Rules:
    // - Every registration is one module the student should have attended.
    // - An absence counts with its share: 50 % absence = half a module.
    // - Absence with a type where CountsTowardsAbsence is false (e.g. school activity) does not count.
    // - Absence without a type counts.
    public static AbsenceSummary Calculate(List<Attendance> registrations)
    {
        double absentModules = 0;

        foreach (var registration in registrations)
        {
            if (registration.Status != AttendanceStatus.Absent)
                continue;

            if (registration.AbsenceType != null && !registration.AbsenceType.CountsTowardsAbsence)
                continue;

            absentModules += (registration.AbsencePercent ?? 100) / 100.0;
        }

        return new AbsenceSummary
        {
            ModuleCount = registrations.Count,
            AbsentModules = absentModules
        };
    }
}
