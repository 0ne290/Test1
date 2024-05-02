using Domain;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class Dao<T>(VendingContext vendingContext) : IDao<T> where T : class
{
    public IEnumerable<T> GetAll() => vendingContext.Set<T>().AsEnumerable();
    
    public IEnumerable<T> GetAllCopies() => vendingContext.Set<T>().AsNoTracking().AsEnumerable();

    public async Task<T?> TryGetByKey(params object[] keyValues) => await vendingContext.Set<T>().FindAsync(keyValues);
    
    /*public async Task<T?> TryGetCopyByKey(params object[] keyValues)
    {
        var entity = await vendingContext.Set<T>().FindAsync(keyValues);

        if (entity != null)
            vendingContext.Entry(entity).State = EntityState.Detached;
        
        return entity;
    }*/

    public async Task SaveChanges() => await vendingContext.SaveChangesAsync();

    public void Dispose() => vendingContext.Dispose();
}
