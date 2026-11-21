namespace GymnasieProjekt.Core.Models;

public enum AttendanceStatus
{
    Present,
    Absent
}

// Tilstedeværelse in the domain model: one student's attendance in one module.
public class Attendance
{
    public int AttendanceId { get; set; }
    public int StudentId { get; set; }
    public int ModuleId { get; set; }
    public AttendanceStatus Status { get; set; }

    // fraværsandel: how much of the module was missed, 1–100 %. Null when present.
    // Null while absent is treated as the whole module (100 %).
    public int? AbsencePercent { get; set; }

    // 0..1 in the domain model: an absence may have no type registered.
    public AbsenceType? AbsenceType { get; set; }
}
