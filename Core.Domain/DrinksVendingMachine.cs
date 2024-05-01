namespace Core.Domain;

public class DrinksVendingMachine : IDisposable
{
    public DrinksVendingMachine(ICoinsDao coinsDao)// Первый параметр - с какими монетами автомат вообще может работать и сколько их будет содержатся на момент создания объекта; Второй параметр - монеты, исключаемые из первого параметра (результирующее множество этой операции станет множеством монет, доступных для пополнения клиентом автомата)
    {
        _coinsDao = coinsDao;
        
        var coins = _coinsDao.GetAll().ToArray();
        
        _innerCoins = coins.ToDictionary(c => c.Denomination);
        
        _outerCoins = coins.Where(c => c.IsRefillable).ToDictionary(c => c.Denomination);
    }

    public IEnumerable<Coin> GetAllCoins() => _coinsDao.GetAll();
    
    public bool DrinkSelected(Drink drink) => _selectedDrinks.Contains(drink);

    public bool DrinkAvailabe(Drink drink) => Rest >= drink.Cost;
    
    public bool CoinIsAllowed(int denomination) => _outerCoins.ContainsKey(denomination);

    public void DepositeCoin(int denomination)
    {
        _outerCoins[denomination].Quantity++;
        Rest += denomination;
    }

    public Dictionary<int, int> BuyDrinks()// Если сдачу выдать невозможно, вернет буфер; если сдача не требуется - пустой словарь
    {
        var change = Rest;
        Rest = 0;

        Dictionary<int, int> res;
        if (change == 0)
        {
            res = new Dictionary<int, int>();
            
            foreach (var drink in _selectedDrinks)
                drink.Quantity--;
        }
        else
            res = GetChange(change);
        
        _selectedDrinks.Clear();
        foreach (var denomination in _outerCoins.Keys)
            _outerCoins[denomination].Quantity = 0;
        
        _coinsDao.UpdateRange(_innerCoins.Select(c => c.Value));

        return res;
    }
    
    private Dictionary<int, int> GetChange(int change)// Жадный алгоритм расчета кол-ва монет для сдачи. Представьте, что монеты с одинаковым номиналом собраны в кучи, лежащие в ряд слева направо. Тогда этот жадный алгоритм будет максимально быстро израсходовать самые левые кучи - не "ВАУ", конечно, но зато просто. Хотя псевдо-оптимизация все же присутствует - представьте также, что на каждый расчет порядок куч всегда определяется случайно
    {
        // К монетам автомата прибавляются монеты буфера
        foreach (var denomination in _outerCoins.Keys)
            _innerCoins[denomination].Quantity += _outerCoins[denomination].Quantity;
        
        var res = new Dictionary<int, int>();

        var shuffledCoins = _innerCoins.Select(c => c.Value).ToArray();
        Random.Shared.Shuffle(shuffledCoins);
        
        foreach(var coin in shuffledCoins)
        {
            var count = change / coin.Denomination;
            
            if (count > coin.Quantity)
                count = coin.Quantity;

            res.Add(coin.Denomination, count);
            
            change -= coin.Denomination * count;

            if (change != 0)
                continue;
            
            foreach (var drink in _selectedDrinks)
                drink.Quantity--;
            
            foreach (var changeCoin in res)
                _innerCoins[changeCoin.Key].Quantity -= changeCoin.Value;
                
            return res;
        }
        
        // Сдачу выдать не получилось - вычитаем от монет автомата монеты буфера
        foreach (var denomination in _outerCoins.Keys)
            _innerCoins[denomination].Quantity -= _outerCoins[denomination].Quantity;
        
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
    
    public void Dispose() => _coinsDao.Dispose();

    public int Rest { get; private set; }

    private readonly ICoinsDao _coinsDao;

    private readonly Dictionary<int, Coin> _innerCoins;// Монеты автомата. Ключ - номинал монеты
    
    private readonly Dictionary<int, Coin> _outerCoins;// Монеты буфера. Клиент может вносить в автомат только те монеты, которые содержатся в буфере. Внесенные монеты переходят из буфера в автомат в тот момент, когда вызывается метод BuyDrinks()

    private readonly HashSet<Drink> _selectedDrinks = new();
}
