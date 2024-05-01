namespace Core.Domain;

public class DrinksVendingMachine : IDisposable
{
    public DrinksVendingMachine(IEnumerable<Coin> coins)
    {
        _innerCoins = new HashSet(coins);
        _outerCoins = new HashSet(coins.Where(c => c.IsRefillable).Select(c => new Coin() { Denomination = c.Denomination, Quantity = 0, IsRefillable = true }));
    }
    
    public bool DrinkSelected(Drink drink) => _selectedDrinks.Contains(drink);

    public bool DrinkAvailabe(Drink drink) => Rest >= drink.Cost;
    
    public bool CoinIsAllowed(Coin coin) => _outerCoins.Contains(coin);

    public void DepositeCoin(Coin coin)
    {
        _outerCoins.TryGetValue(coin, out var outerCoin);
        outerCoin.Quantity++;
        Rest += denomination;
    }

    public IEnumerable<Coin> BuyDrinks()// Если сдачу выдать невозможно, вернет буфер; если сдача не требуется - пустую коллекцию монет
    {
        var change = Rest;
        Rest = 0;

        IEnumerable<Coin> res;
        if (change == 0)
        {
            res = Array.Empty<Coin>();
            
            foreach (var drink in _selectedDrinks)
                drink.Quantity--;
        }
        else
            res = GetChange(change);
        
        _selectedDrinks.Clear();
        foreach (var coin in _outerCoins)
            coin.Quantity = 0;

        return res;
    }
    
    private IEnumerable<Coin> GetChange(int change)// Жадный алгоритм расчета кол-ва монет для сдачи. Представьте, что монеты с одинаковым номиналом собраны в кучи, лежащие в ряд слева направо. Тогда этот жадный алгоритм будет максимально быстро израсходовать самые левые кучи - не "ВАУ", конечно, но зато просто. Хотя псевдо-оптимизация все же присутствует - представьте также, что на каждый расчет порядок куч всегда определяется случайно
    {
        // К монетам автомата прибавляются монеты буфера
        foreach (var outerCoin in _outerCoins)
        {
            _innerCoins.TryGetValue(outerCoin, out var innerCoin);
            innerCoin.Quantity += outerCoin.Quantity;
        }
        
        var res = new List<Coin>();

        var changeCoins = _innerCoins.Select(c = new Coin() { Denomination = c.Denomination, Quantity = c.Quantity, IsRefillable = c.IsRefillable }).ToArray();
        Random.Shared.Shuffle(changeCoins);
        
        foreach(var coin in changeCoins)
        {
            var count = change / coin.Denomination;
            
            if (count < coin.Quantity)
                coin.Quantity = count;

            res.Add(coin);
            
            change -= coin.Denomination * coin.Quantity;

            if (change != 0)
                continue;
            
            foreach (var drink in _selectedDrinks)
                drink.Quantity--;
            
            foreach (var changeCoin in res)
            {
                _innerCoins.TryGetValue(changeCoin, out var innerCoin);
                innerCoin.Quantity -= changeCoin.Value;
            }
                
            return res;
        }
        
        // Сдачу выдать не получилось - вычитаем от монет автомата монеты буфера
        foreach (var outerCoin in _outerCoins)
        {
            _innerCoins.TryGetValue(outerCoin, out var innerCoin);
            innerCoin.Quantity -= outerCoin.Quantity;
        }
        
        return _outerCoins.ToDictionary(c => c.Key, c => c.Value.Quantity);
    }

    public void UnselectDrink(Drink drink)
    {
        Rest += drink.Cost;
        _selectedDrinks.Remove(drink);
    }

    public void SelectDrink(Drink drink)
    {
        Rest -= drink.Cost;
        _selectedDrinks.Add(drink);
    }

    public int Rest { get; private set; }

    private readonly HashSet<Coin> _innerCoins;// Монеты автомата
    
    private readonly HashSet<Coin> _outerCoins;// Монеты буфера. Клиент может вносить в автомат только те монеты, которые содержатся в буфере. Внесенные монеты переходят из буфера в автомат в тот момент, когда вызывается метод BuyDrinks()

    private readonly HashSet<Drink> _selectedDrinks = new();
}
