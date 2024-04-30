using Core.Domain;

namespace Core.Application;

public class DrinksVendingMachineInteractor(DrinksVendingMachine vending, IDrinksDao drinksDao) : IDisposable
{
    public IEnumerable<Drink> GetAllDrinks() => drinksDao.GetAll();

    public int DepositeCoin(int denomination)
    {
        if (!vending.CoinIsAllowed(denomination))
            throw new Exception($"Coin {denomination} is not available for replenishment.");
        
        vending.DepositeCoin(denomination);

        DepositedAmount += denomination;
        return DepositedAmount;
    }

    public async Task<bool> ChooseDrink(int drinkKey)
    {
        var drink = await drinksDao.TryGetByKey(drinkKey);

        if (drink == null)
            throw new Exception($"This machine does not contain a drink with the {drinkKey} key.");

        if (drink.Quantity < 1)
            throw new Exception($"Drink {drink.Name} is out of stock.");

        if (!vending.DrinkAvailabe(drink))
            throw new Exception($"Cost of the drink {drink.Name} is more than the deposited amount.");

        if (vending.DrinkSelected(drink))
        {
            vending.UnselectDrink(drink);
            return false;
        }

        vending.SelectDrink(drink);
        return true;
    }
    
    public Dictionary<int, int> BuyDrinks()
    {
        DepositedAmount = 0;

        return vending.BuyDrinks();
    }
    
    public void ResetSelection() => vending.ResetSelection();
    
    public void Dispose()
    {
        vending.Dispose();
        drinksDao.Dispose();
    }
    
    public int Rest => vending.Rest;

    public int DepositedAmount { get; private set; }
}