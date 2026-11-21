namespace GymnasieProjekt.Core.Models;

// Modul in the domain model: one lesson held by a team.
public class Module
{
    public int ModuleId { get; set; }
    public int TeamId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int ModuleNumber { get; set; }    // modul 1, 2, 3 or 4 of the day
}
