namespace GymnasieProjekt.Core.Repositories;

// Common methods for all repositories. Only read methods for now; write methods
// (e.g. Add) can be added here when the database is filled with data from Lectio.
public interface IRepository<T>
{
    List<T> GetAll();
    T? GetById(int id);
}
