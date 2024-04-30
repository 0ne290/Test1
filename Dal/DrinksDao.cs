using Core.Application;
using Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class DrinksDao(VendingContext vendingContext) : IDrinksDao
{
    public IEnumerable<Drink> GetAll() => vendingContext.Drinks.AsNoTracking();

    public async Task<Drink?> TryGetByKey(int key) => await vendingContext.Drinks.FindAsync(key);

    public void Dispose() => vendingContext.Dispose();
}