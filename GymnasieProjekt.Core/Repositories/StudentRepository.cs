using GymnasieProjekt.Core.Models;
using Microsoft.Data.SqlClient;

namespace GymnasieProjekt.Core.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly string _connectionString;

    public StudentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Student> GetAll()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(
            "SELECT StudentId, FirstName, LastName, ClassName FROM Student ORDER BY LastName, FirstName",
            connection);

        return ReadStudents(command);
    }

    public Student? GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(
            "SELECT StudentId, FirstName, LastName, ClassName FROM Student WHERE StudentId = @StudentId",
            connection);
        command.Parameters.AddWithValue("@StudentId", id);

        return ReadStudents(command).FirstOrDefault();
    }

    public List<Student> GetByClass(string className)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(
            "SELECT StudentId, FirstName, LastName, ClassName FROM Student WHERE ClassName = @ClassName ORDER BY LastName, FirstName",
            connection);
        command.Parameters.AddWithValue("@ClassName", className);

        return ReadStudents(command);
    }

    // Shared mapping from database rows to Student objects
    private static List<Student> ReadStudents(SqlCommand command)
    {
        var students = new List<Student>();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            students.Add(new Student
            {
                StudentId = (int)reader["StudentId"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                ClassName = (string)reader["ClassName"]
            });
        }

        return students;
    }
}
