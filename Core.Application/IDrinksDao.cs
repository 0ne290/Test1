using Core.Domain;

namespace Core.Application;

public interface IDrinksDao : IDisposable
{
    IEnumerable<Drink> GetAll();

    Task<Drink?> TryGetByKey(int key);

    Task SaveChanges();
}
