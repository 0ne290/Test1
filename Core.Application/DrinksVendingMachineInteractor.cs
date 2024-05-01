using Core.Domain;

namespace Core.Application;

public class DrinksVendingMachineInteractor(DrinksVendingMachine vending, IDrinksDao drinksDao) : IDisposable
{
    public IEnumerable<Drink> GetAllDrinks() => drinksDao.GetAll();
    
    public IEnumerable<Coin> GetAllCoins() => vending.GetAllCoins();

    public void DepositeCoin(int denomination)
    {
        if (!vending.CoinIsAllowed(denomination))
            throw new Exception($"Coin {denomination} is not available for replenishment.");
        
        vending.DepositeCoin(denomination);

        DepositedAmount += denomination;
    }

    public async Task<bool> ChooseDrink(int drinkKey)
    {
        var drink = await drinksDao.TryGetByKey(drinkKey);

        if (drink == null)
            throw new Exception($"This machine does not contain a drink with the {drinkKey} key.");

        if (vending.DrinkSelected(drink))// Вернуть false и снять выделение с напитка, если он уже выделен
        {
            vending.UnselectDrink(drink);
            return false;
        }
        
        if (drink.Quantity < 1 || !vending.DrinkAvailabe(drink))// Вернуть false, напиток выделить невозможно
            return false;

        vending.SelectDrink(drink);
        return true;
    }
    
    public Dictionary<int, int> BuyDrinks()
    {
        DepositedAmount = 0;

        return vending.BuyDrinks();
    }
    
    public void Dispose()
    {
        vending.Dispose();
        drinksDao.Dispose();
    }
    
    public int Rest => vending.Rest;

    public int DepositedAmount { get; private set; }
}