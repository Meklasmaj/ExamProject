using GymnasieProjekt.Core.Models;

namespace GymnasieProjekt.Core.Repositories;

public interface IStudentRepository : IReadRepository<Student>
{
    List<Student> GetByClass(string className);
}
