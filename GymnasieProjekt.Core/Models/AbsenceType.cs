namespace GymnasieProjekt.Core.Models;

// Fraværstype in the domain model, e.g. "Sygdom" or "Skolerelateret aktivitet".
public class AbsenceType
{
    public int AbsenceTypeId { get; set; }
    public string Name { get; set; } = "";

    // medregnes [ja/nej]: whether this type counts towards the student's absence
    public bool CountsTowardsAbsence { get; set; }
}
