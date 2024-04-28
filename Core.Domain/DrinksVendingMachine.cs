namespace Core.Domain;

public class DrinksVendingMachine
{
    public DrinksVendingMachine(Dictionary<int, int> coins)
    {
        _innerCoins = coins;
        _outerCoins = new Dictionary<int, int>(_innerCoins);

        foreach (var denomination in _outerCoins.Keys)
            _outerCoins[denomination] = 0;
    }
    
    public bool DrinkSelected(Drink drink) => _selectedDrinks.Contains(drink);

    public int GetCapacityOuterCoins() => _outerCoins.Keys.Sum(denomination => denomination * _outerCoins[denomination]);

    public (int OneCoinsNumber, int TwoCoinsNumber, int FiveCoinsNumber, int TenCoinsNumber) BuyDrinks()
    {
        
    }
    
    private Dictionary<int, int> GetChange(int change)// Жадный алгоритм расчета кол-ва монет для сдачи. Представьте, что монеты с одинаковым номиналом собраны в кучи, лежащие в ряд слева направо. Тогда этот жадный алгоритм будет максимально быстро израсходовать самые левые кучи - не "ВАУ", конечно, но зато просто. Хотя псевдо-оптимизация все же присутствует - представьте, что на каждый расчет порядок куч всегда определяется случайно
    {
        var res = new Dictionary<int, int>();

        var shuffledCoins = _innerCoins.Select().ToArray();
        Random.Shared.Shuffle(shuffledCoins);
        
        foreach(var coin in shuffledCoins)
        {
            var count = change / coin.Key;
            
            if (count > coin.Value)
                count = coin.Value;

            res.Add(coin.Key, count);
            
            change = change % coin.Key;
            
            if (change == 0)
            {
                foreach (var changeCoin in res)
                    _innerCoins[changeCoin.Key] -= changeCoin.Value();
                
                return res;
            }
        }

        return new Dictionary<int, int>();// Сдачу выдать невозможно
    }
    
    public void ResetSelection()
    {
        foreach (var drink in _selectedDrinks.ToArray())
            UnselectDrink(drink);
    }

    public void UnselectDrink(Drink drink)
    {
        DepositedAmount += drink.Cost;
        _selectedDrinks.Remove(drink);
    }

    public void SelectDrink(Drink drink)
    {
        DepositedAmount -= drink.Cost;
        _selectedDrinks.Add(drink);
    }

    public int DepositedAmount { get; set; }

    private readonly Dictionary<int, int> _innerCoins;// Ключ - номинал монеты, значение по этому ключу - кол-во этих монет
    
    private readonly Dictionary<int, int> _outerCoins;

    private readonly HashSet<Drink> _selectedDrinks = new();
}
