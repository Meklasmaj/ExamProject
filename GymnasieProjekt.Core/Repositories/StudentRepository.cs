using GymnasieProjekt.Core.Models;
using Microsoft.Data.SqlClient;

namespace GymnasieProjekt.Core.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly string _connectionString;

    // Shared start of every query: a student together with their class
    private const string SelectStudents = @"
        SELECT s.StudentId, s.FirstName, s.LastName,
               c.SchoolClassId, c.Name AS ClassName, c.Year AS ClassYear
        FROM Student s
        JOIN SchoolClass c ON c.SchoolClassId = s.SchoolClassId";

    public StudentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Student> GetAll()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(
            SelectStudents + " ORDER BY s.LastName, s.FirstName",
            connection);

        return ReadStudents(command);
    }

    public Student? GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(
            SelectStudents + " WHERE s.StudentId = @StudentId",
            connection);
        command.Parameters.AddWithValue("@StudentId", id);

        return ReadStudents(command).FirstOrDefault();
    }

    public List<Student> GetByClass(string className)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(
            SelectStudents + " WHERE c.Name = @ClassName ORDER BY s.LastName, s.FirstName",
            connection);
        command.Parameters.AddWithValue("@ClassName", className);

        return ReadStudents(command);
    }

    public List<Student> SearchByName(string searchText)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand(
            SelectStudents + @"
            WHERE s.FirstName + ' ' + s.LastName LIKE @Search
            ORDER BY s.FirstName, s.LastName",
            connection);
        // The % wildcards are added here, never glued into the SQL text (SQL injection)
        command.Parameters.AddWithValue("@Search", "%" + searchText + "%");

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
                SchoolClass = new SchoolClass
                {
                    SchoolClassId = (int)reader["SchoolClassId"],
                    Name = (string)reader["ClassName"],
                    Year = (int)reader["ClassYear"]
                }
            });
        }

        return students;
    }
}
