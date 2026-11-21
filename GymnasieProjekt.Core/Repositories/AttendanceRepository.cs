using GymnasieProjekt.Core.Models;
using Microsoft.Data.SqlClient;

namespace GymnasieProjekt.Core.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly string _connectionString;

    // LEFT JOIN because an absence may have no type (0..1 in the domain model)
    private const string SelectAttendance = @"
        SELECT a.AttendanceId, a.StudentId, a.ModuleId, a.Status, a.AbsencePercent,
               t.AbsenceTypeId, t.Name AS AbsenceTypeName, t.CountsTowardsAbsence
        FROM Attendance a
        LEFT JOIN AbsenceType t ON t.AbsenceTypeId = a.AbsenceTypeId";

    public AttendanceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Attendance> GetAll()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(SelectAttendance, connection);

        return ReadAttendance(command);
    }

    public Attendance? GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(
            SelectAttendance + " WHERE a.AttendanceId = @AttendanceId",
            connection);
        command.Parameters.AddWithValue("@AttendanceId", id);

        return ReadAttendance(command).FirstOrDefault();
    }

    public List<Attendance> GetByStudent(int studentId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(
            SelectAttendance + " WHERE a.StudentId = @StudentId",
            connection);
        command.Parameters.AddWithValue("@StudentId", studentId);

        return ReadAttendance(command);
    }

    private static List<Attendance> ReadAttendance(SqlCommand command)
    {
        var result = new List<Attendance>();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var attendance = new Attendance
            {
                AttendanceId = (int)reader["AttendanceId"],
                StudentId = (int)reader["StudentId"],
                ModuleId = (int)reader["ModuleId"],
                Status = Enum.Parse<AttendanceStatus>((string)reader["Status"]),
                // NULL in the database becomes DBNull in C#, so check before casting
                AbsencePercent = reader["AbsencePercent"] is DBNull ? null : (int)reader["AbsencePercent"]
            };

            if (reader["AbsenceTypeId"] is not DBNull)
            {
                attendance.AbsenceType = new AbsenceType
                {
                    AbsenceTypeId = (int)reader["AbsenceTypeId"],
                    Name = (string)reader["AbsenceTypeName"],
                    CountsTowardsAbsence = (bool)reader["CountsTowardsAbsence"]
                };
            }

            result.Add(attendance);
        }

        return result;
    }
}
