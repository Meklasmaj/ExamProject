using GymnasieProjekt.Core.Models;
using GymnasieProjekt.Core.Repositories;
using GymnasieProjekt.Core.Services;

namespace GymnasieProjekt.Tests;

// Tests the absence rules in AbsenceService.Calculate, and that GetTotalAbsence
// uses the repository. No database is needed.
[TestClass]
public class AbsenceServiceTests
{
    private static readonly AbsenceType Sickness = new() { AbsenceTypeId = 1, Name = "Sygdom", CountsTowardsAbsence = true };
    private static readonly AbsenceType SchoolActivity = new() { AbsenceTypeId = 3, Name = "Skolerelateret aktivitet", CountsTowardsAbsence = false };

    // ----- Helpers -----

    private static Attendance Present() => new() { Status = AttendanceStatus.Present };

    private static Attendance Absent(int? percent = null, AbsenceType? type = null) =>
        new() { Status = AttendanceStatus.Absent, AbsencePercent = percent, AbsenceType = type };

    // ----- Calculate -----

    [TestMethod]
    public void Calculate_NoRegistrations_ReturnsZero()
    {
        var summary = AbsenceService.Calculate(new List<Attendance>());

        Assert.AreEqual(0, summary.ModuleCount);
        Assert.AreEqual(0, summary.AbsentModules);
        Assert.AreEqual(0, summary.AbsencePercent);   // no division by zero
    }

    [TestMethod]
    public void Calculate_AllPresent_NoAbsence()
    {
        var summary = AbsenceService.Calculate([Present(), Present(), Present()]);

        Assert.AreEqual(3, summary.ModuleCount);
        Assert.AreEqual(0, summary.AbsentModules);
    }

    [TestMethod]
    public void Calculate_AbsenceWithoutPercent_CountsAsWholeModule()
    {
        var summary = AbsenceService.Calculate([Absent(), Present()]);

        Assert.AreEqual(1, summary.AbsentModules);
    }

    [TestMethod]
    public void Calculate_AbsencePercent50_CountsAsHalfModule()
    {
        var summary = AbsenceService.Calculate([Absent(percent: 50), Present()]);

        Assert.AreEqual(0.5, summary.AbsentModules);
    }

    [TestMethod]
    public void Calculate_AbsenceWithoutType_Counts()
    {
        var summary = AbsenceService.Calculate([Absent(type: null)]);

        Assert.AreEqual(1, summary.AbsentModules);
    }

    [TestMethod]
    public void Calculate_TypeThatCounts_Counts()
    {
        var summary = AbsenceService.Calculate([Absent(type: Sickness)]);

        Assert.AreEqual(1, summary.AbsentModules);
    }

    [TestMethod]
    public void Calculate_TypeThatDoesNotCount_IsIgnoredButModuleStillCounts()
    {
        var summary = AbsenceService.Calculate([Absent(type: SchoolActivity), Present()]);

        Assert.AreEqual(0, summary.AbsentModules);
        Assert.AreEqual(2, summary.ModuleCount);
    }

    [TestMethod]
    public void Calculate_MixedRegistrations_GivesCorrectPercent()
    {
        // 1 + 0.5 + 0 (school activity) + 0 (present) = 1.5 of 4 modules = 37.5 %
        var summary = AbsenceService.Calculate(
            [Absent(type: Sickness), Absent(percent: 50), Absent(type: SchoolActivity), Present()]);

        Assert.AreEqual(4, summary.ModuleCount);
        Assert.AreEqual(1.5, summary.AbsentModules);
        Assert.AreEqual(37.5, summary.AbsencePercent, 0.0001);
    }

    // ----- GetTotalAbsence -----

    [TestMethod]
    public void GetTotalAbsence_UsesRegistrationsForTheGivenStudent()
    {
        var repository = new FakeAttendanceRepository([Absent(), Present(), Present(), Present()]);
        var service = new AbsenceService(repository);

        var summary = service.GetTotalAbsence(studentId: 7);

        Assert.AreEqual(7, repository.RequestedStudentId);
        Assert.AreEqual(4, summary.ModuleCount);
        Assert.AreEqual(25, summary.AbsencePercent, 0.0001);
    }

    // A stand-in for the database. AbsenceService depends on the interface,
    // so the test can give it this instead of AttendanceRepository.
    private class FakeAttendanceRepository : IAttendanceRepository
    {
        private readonly List<Attendance> _registrations;

        public FakeAttendanceRepository(List<Attendance> registrations)
        {
            _registrations = registrations;
        }

        public int? RequestedStudentId { get; private set; }

        public List<Attendance> GetByStudent(int studentId)
        {
            RequestedStudentId = studentId;
            return _registrations;
        }

        public List<Attendance> GetAll() => _registrations;

        public Attendance? GetById(int id) => null;
    }
}
