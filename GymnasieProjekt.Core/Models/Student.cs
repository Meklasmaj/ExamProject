namespace GymnasieProjekt.Core.Models;

public class Student
{
    public int StudentId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string ClassName { get; set; } = "";

    public string FullName => $"{FirstName} {LastName}";
}
