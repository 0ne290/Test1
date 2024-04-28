namespace Core.Domain;

public class DrinksVendingMachine
{
    public DrinksVendingMachine(IDrinksDao drinks)
    {
        _drinks = drinks;
    }
    
    public void ChooseDrink(int drinkKey)
    {
        if (_drinks.TryGetByKey(drinkKey) == null)
            throw new Exception($"This machine does not contain a drink with the {drinkKey} key.");
    }
    
    public int OneCoinsNumber { get; set; }
    
    public int TwoCoinsNumber { get; set; }
    
    public int FiveCoinsNumber { get; set; }
    
    public int TenCoinsNumber { get; set; }
    
    public int DepositedAmount { get; set; }

    private readonly IDrinksDao _drinks;
}