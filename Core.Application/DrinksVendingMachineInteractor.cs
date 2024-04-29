using Core.Domain;

namespace Core.Application;

public class DrinksVendingMachineInteractor
{
    public DrinksVendingMachineInteractor(DrinksVendingMachine vending, IDrinksDao drinks)
    {
        _vending = vending;
        _drinks = drinks;
    }

    public void DepositeCoin(int denomination) => _vending.DepositedAmount += denomination;
    
    public void ChooseDrink(int drinkKey)
    {
        var drink = _drinks.TryGetByKey(drinkKey);
        
        if (drink == null)
            throw new Exception($"This machine does not contain a drink with the {drinkKey} key.");
        
        if (drink.Quantity < 1)
            throw new Exception($"Drink {drink.Name} is out of stock.");
        
        if (!_vending.DrinkAvailabe(drink))
            throw new Exception($"Cost of the drink {drink.Name} is more than the deposited amount.");
        
        if (_vending.DrinkSelected(drink))
            _vending.UnselectDrink(drink);
        else
            _vending.UnselectDrink(drink);
    }

    private readonly DrinksVendingMachine _vending;
    
    private readonly IDrinksDao _drinks;
}