namespace Core.Domain;

public class DrinksVendingMachine
{
    public DrinksVendingMachine(Dictionary<int, int> coins, HashSet<int> prohibitedCoins)// Первый параметр - с какими монетами автомат вообще может работать и сколько их будет содержатся на момент создания объекта; Второй параметр - монеты, исключаемые из первого параметра (результирующее множество этой операции станет множеством монет, доступных для пополнения клиентом автомата)
    {
        _innerCoins = coins.ToDictionary();
        _outerCoins = new Dictionary<int, int>(_innerCoins.Count);

        foreach (var denomination in _outerCoins.Keys)
            if (!prohibitedCoins.Contains(denomination))
                _outerCoins.Add(denomination, 0);
        
        _outerCoins.TrimExcess();
    }
    
    public bool DrinkSelected(Drink drink) => _selectedDrinks.Contains(drink);

    public bool DrinkAvailabe(Drink drink) => _capacityOuterCoins >= drink.Cost;
    
    public bool CoinIsAllowed(int denomination) => _outerCoins.ContainsKey(denomination);

    public void DepositeCoin(int denomination)
    {
        _outerCoins[denomination]++;
        _capacityOuterCoins += denomination;
    }

    public Dictionary<int, int> BuyDrinks()
    {
        // Внесенные клиентом монеты "уходят в собственность автомата"
        foreach (var denomination in _outerCoins.Keys)
        {
            _innerCoins[denomination] += _outerCoins[denomination];
            _outerCoins[denomination] = 0;
        }

        _capacityOuterCoins = 0;
        
        
    }
    
    private Dictionary<int, int> GetChange(int change)// Жадный алгоритм расчета кол-ва монет для сдачи. Представьте, что монеты с одинаковым номиналом собраны в кучи, лежащие в ряд слева направо. Тогда этот жадный алгоритм будет максимально быстро израсходовать самые левые кучи - не "ВАУ", конечно, но зато просто. Хотя псевдо-оптимизация все же присутствует - представьте также, что на каждый расчет порядок куч всегда определяется случайно
    {
        var res = new Dictionary<int, int>();

        var shuffledCoins = _innerCoins.Select(c => c).ToArray();
        Random.Shared.Shuffle(shuffledCoins);
        
        foreach(var coin in shuffledCoins)
        {
            var count = change / coin.Key;
            
            if (count > coin.Value)
                count = coin.Value;

            res.Add(coin.Key, count);
            
            change %= coin.Key;

            if (change != 0)
                continue;
            
            foreach (var changeCoin in res)
                _innerCoins[changeCoin.Key] -= changeCoin.Value;
                
            return res;
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
        _capacityOuterCoins += drink.Cost;
        _selectedDrinks.Remove(drink);
    }

    public void SelectDrink(Drink drink)
    {
        _capacityOuterCoins -= drink.Cost;
        _selectedDrinks.Add(drink);
    }

    private int _capacityOuterCoins;

    private readonly Dictionary<int, int> _innerCoins;// Монеты автомата. Ключ - номинал монеты, значение по этому ключу - кол-во этих монет
    
    private readonly Dictionary<int, int> _outerCoins;// Монеты буфера. Клиент может вносить в автомат только те монеты, которые содержатся в буфере. Внесенные монеты переходят из буфера в автомат в тот момент, когда вызывается метод BuyDrinks()

    private readonly HashSet<Drink> _selectedDrinks = new();
}
