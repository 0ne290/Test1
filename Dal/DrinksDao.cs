using Core.Application;
using Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class DrinksDao : IDrinksDao
{
    public DrinksDao(VendingContext vendingContext) => _vendingContext = vendingContext;

    public async Task Create(Drink drink) => await _vendingContext.Drinks.AddAsync(drink);

    public IEnumerable<Drink> GetAll() => _vendingContext.Drinks.AsNoTracking();

    public async Task<Drink?> TryGetByKey(int key) => await _vendingContext.Drinks.FindAsync(key);

    public void Update(Drink drink) => _vendingContext.Drinks.Update(drink);

    public void Remove(Drink drink) => _vendingContext.Drinks.Remove(drink);

    public async Task SaveChanges() => await _vendingContext.SaveChangesAsync();

    public void Dispose() => _vendingContext.Dispose();

    private readonly VendingContext _vendingContext;
}