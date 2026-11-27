using GymnasieProjekt.Core.Models;

namespace GymnasieProjekt.Core.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    List<Student> GetByClass(string className);

    // Students whose full name contains the search text, e.g. "ann" finds "Anna Jensen"
    List<Student> SearchByName(string searchText);
}
