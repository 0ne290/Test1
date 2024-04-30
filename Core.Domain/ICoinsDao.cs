namespace Core.Domain;

public interface ICoinsDao : IDisposable
{
    IEnumerable<Coin> GetAll();

    Task UpdateRange(IEnumerable<Coin> coins);
}