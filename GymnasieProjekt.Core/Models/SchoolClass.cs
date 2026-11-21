namespace GymnasieProjekt.Core.Models;

// Klasse in the domain model. Named SchoolClass because "Class" is confusing in C#.
public class SchoolClass
{
    public int SchoolClassId { get; set; }
    public string Name { get; set; } = "";   // klassenavn, e.g. "1.a"
    public int Year { get; set; }            // årgang, e.g. 2026
}
