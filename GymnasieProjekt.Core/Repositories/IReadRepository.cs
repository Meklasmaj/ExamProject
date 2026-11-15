namespace GymnasieProjekt.Core.Repositories;

// Read-only on purpose: the data belongs to Lectio, so the app never writes it back.
public interface IReadRepository<T>
{
    List<T> GetAll();
    T? GetById(int id);
}
