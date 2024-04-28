using Core.Domain;

namespace Core.Application;

public interface IDrinksDao
{
    bool Create(Drink drink);

    IEnumerable<Drink> GetAll();

    Drink? TryGetByKey(int key);

    void Update(Drink drink);

    void Remove(Drink drink);

    bool SaveChanges();
}