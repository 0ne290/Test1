using Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public class CoinsDao(VendingContext vendingContext) : ICoinsDao
{
    public IEnumerable<Coin> GetAll() => vendingContext.Coins.AsNoTracking();

    public async Task UpdateRange(IEnumerable<Coin> coins)
    {
        vendingContext.Coins.UpdateRange(coins);

        await vendingContext.SaveChangesAsync();
    }

    public void Dispose() => vendingContext.Dispose();
}