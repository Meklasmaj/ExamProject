namespace GymnasieProjekt.Core.Models;

// Hold in the domain model: a group of students taking a subject together.
public class Team
{
    public int TeamId { get; set; }
    public string Name { get; set; } = "";   // holdnavn, e.g. "1.a Dansk"
    public int Year { get; set; }            // årstal
}
