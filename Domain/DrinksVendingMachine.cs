namespace Domain;

public class DrinksVendingMachine : IDisposable
{
    public DrinksVendingMachine(IDao<Drink> drinksDao, IDao<Coin> coinsDao)
    {
        _drinksDao = drinksDao;
        _coinsDao = coinsDao;
        
        _innerCoins = _coinsDao.GetAll().ToDictionary(c => c.Denomination);
        _outerCoins = _coinsDao.GetAllCopies().Where(c => c.IsRefillable).ToDictionary(c => c.Denomination);
    }
    
    public IEnumerable<Drink> GetAllDrinks() => _drinksDao.GetAllCopies();
    
    public IEnumerable<Coin> GetAllCoins() => _coinsDao.GetAllCopies();

    public void DepositeCoin(int denomination)
    {
        if (!CoinIsAllowed(denomination))
            throw new Exception($"Coin {denomination} is not available for replenishment.");

        _outerCoins[denomination].Quantity++;
        
        Rest += denomination;
        DepositedAmount += denomination;
    }
    
    private bool CoinIsAllowed(int denomination) => _outerCoins.ContainsKey(denomination);

    public async Task<bool> ChooseDrink(int drinkKey)
    {
        var drink = await _drinksDao.TryGetByKey(drinkKey);

        if (drink == null)
            throw new Exception($"This machine does not contain a drink with the {drinkKey} key.");

        if (DrinkSelected(drink))// Вернуть false и снять выделение с напитка, если он уже выделен
        {
            UnselectDrink(drink);
            return false;
        }
        
        if (drink.Quantity < 1 || !DrinkAvailabe(drink))// Вернуть false, напиток выделить невозможно
            return false;

        SelectDrink(drink);
        return true;
    }
    
    private bool DrinkSelected(Drink drink) => _selectedDrinks.Contains(drink);
    
    private bool DrinkAvailabe(Drink drink) => Rest >= drink.Cost;
    
    private void UnselectDrink(Drink drink)
    {
        Rest += drink.Cost;
        _selectedDrinks.Remove(drink);
    }

    private void SelectDrink(Drink drink)
    {
        Rest -= drink.Cost;
        _selectedDrinks.Add(drink);
    }
    
    public async Task<Dictionary<int, int>> BuyDrinks()
    {
        DepositedAmount = 0;

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
        foreach (var coin in _outerCoins)
            coin.Value.Quantity = 0;

        await _drinksDao.SaveChanges();
        await _coinsDao.SaveChanges();

        return res;
    }
    
    private Dictionary<int, int> GetChange(int change)// Жадный алгоритм расчета кол-ва монет для сдачи. Представьте, что монеты с одинаковым номиналом собраны в кучи, лежащие в ряд слева направо. Тогда этот жадный алгоритм будет максимально быстро израсходовать самые левые кучи - не "ВАУ", конечно, но зато просто. Хотя псевдо-оптимизация все же присутствует - представьте также, что на каждый расчет порядок куч всегда определяется случайно
    {
        // К монетам автомата прибавляются монеты буфера
        foreach (var outerCoin in _outerCoins)
            _innerCoins[outerCoin.Key].Quantity += outerCoin.Value.Quantity;
        
        var res = new Dictionary<int, int>();

        var changeCoins = _innerCoins.Select(c => c.Value).ToArray();
        Random.Shared.Shuffle(changeCoins);
        
        foreach(var coin in changeCoins)
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
        foreach (var outerCoin in _outerCoins)
            _innerCoins[outerCoin.Key].Quantity -= outerCoin.Value.Quantity;
        
        return _outerCoins.ToDictionary(c => c.Key, c => c.Value.Quantity);
    }
    
    public void Dispose()
    {
        _drinksDao.Dispose();
        _coinsDao.Dispose();
    }
    
    public int Rest { get; private set; }

    public int DepositedAmount { get; private set; }

    private readonly IDao<Drink> _drinksDao;

    private readonly IDao<Coin> _coinsDao;
    
    private readonly Dictionary<int, Coin> _innerCoins;// Монеты автомата
    
    private readonly Dictionary<int, Coin> _outerCoins;// Монеты буфера. Клиент может вносить в автомат только те монеты, которые содержатся в буфере. Внесенные монеты переходят из буфера в автомат в тот момент, когда вызывается метод BuyDrinks()

    private readonly HashSet<Drink> _selectedDrinks = new();
}
