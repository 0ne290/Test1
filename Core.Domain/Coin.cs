namespace Core.Domain;

public class Coin
{
    public Coin(int denomination, int quantity)
    {
        Denomination = denomination;
        Quantity = quantity;
    }

    public int Denomination { get; }
    
    public int Quantity { get; set; }
}