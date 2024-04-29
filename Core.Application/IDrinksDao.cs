using Core.Domain;

namespace Core.Application;

public interface IDrinksDao : IDisposable
{
    Task Create(Drink drink);

    IEnumerable<Drink> GetAll();

    Task<Drink?> TryGetByKey(int key);

    void Update(Drink drink);

    void Remove(Drink drink);

    Task SaveChanges();
}