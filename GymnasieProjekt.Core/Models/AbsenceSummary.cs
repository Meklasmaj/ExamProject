namespace GymnasieProjekt.Core.Models;

// Result of an absence calculation for one student. Not stored in the database.
public class AbsenceSummary
{
    public int ModuleCount { get; set; }        // modules the student was registered on
    public double AbsentModules { get; set; }   // counted absence in modules, e.g. 11.5
    public double AbsencePercent => ModuleCount == 0 ? 0 : AbsentModules / ModuleCount * 100;
}
