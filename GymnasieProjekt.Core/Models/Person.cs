namespace GymnasieProjekt.Core.Models;

// Person in the domain model. Base class for Student (and later Employee).
public abstract class Person
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";

    public string FullName => $"{FirstName} {LastName}";
}
