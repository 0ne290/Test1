using Core.Domain;

namespace Core.Application;

public class DrinksVendingMachineInteractor : IDisposable
{
    public DrinksVendingMachineInteractor(DrinksVendingMachine vending, IDrinksDao drinks)
    {
        _vending = vending;
        _drinks = drinks;
    }

    public IEnumerable<Drink> GetAllDrinks() => _drinks.GetAll();

    public int DepositeCoin(int denomination)
    {
        if (!_vending.CoinIsAllowed(denomination))
            throw new Exception($"Coin {denomination} is not available for replenishment.");
        
        _vending.DepositeCoin(denomination);

        DepositedAmount += denomination;
        return DepositedAmount;
    }

    public async Task<bool> ChooseDrink(int drinkKey)
    {
        var drink = await _drinks.TryGetByKey(drinkKey);

        if (drink == null)
            throw new Exception($"This machine does not contain a drink with the {drinkKey} key.");

        if (drink.Quantity < 1)
            throw new Exception($"Drink {drink.Name} is out of stock.");

        if (!_vending.DrinkAvailabe(drink))
            throw new Exception($"Cost of the drink {drink.Name} is more than the deposited amount.");

        if (_vending.DrinkSelected(drink))
        {
            _vending.UnselectDrink(drink);
            return false;
        }

        _vending.SelectDrink(drink);
        return true;
    }
    
    public Dictionary<int, int> BuyDrinks()
    {
        DepositedAmount = 0;

        return _vending.BuyDrinks();
    }
    
    public void ResetSelection() => _vending.ResetSelection();
    
    public void Dispose() => _drinks.Dispose();
    
    public int Rest => _vending.Rest;

    public int DepositedAmount { get; private set; }

    private readonly DrinksVendingMachine _vending;
    
    private readonly IDrinksDao _drinks;
}