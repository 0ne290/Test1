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
    
    private int GetChange(int change, int[] getChangeResults)   
    {
        var minCoins = change;

        if (_outerCoins.ContainsKey(minCoins))
        {
            getChangeResults[change] = 1;
            return 1;
        }
        
        if (getChangeResults[change] != 0)
            return getChangeResults[change];
        
        foreach (var value in values.Where(x => x <= change))
        {
            int numCoins = 1 + GetChange(values, change - value);
            if (numCoins < minCoins)
            {
                minCoins = numCoins;
                getChangeResults[change] = minCoins;
            }
        }

        return minCoins;
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

    private readonly Dictionary<int, int> _innerCoins;
    
    private readonly Dictionary<int, int> _outerCoins;

    private readonly HashSet<Drink> _selectedDrinks = new();
}