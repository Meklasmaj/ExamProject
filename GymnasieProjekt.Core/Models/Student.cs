namespace GymnasieProjekt.Core.Models;

// Elev in the domain model.
public class Student : Person
{
    public int StudentId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    // Shown in the search dropdown, e.g. "Anna Jensen (1.a)"
    public string DisplayName => SchoolClass == null ? FullName : $"{FullName} ({SchoolClass.Name})";
}
