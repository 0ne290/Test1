using System.ComponentModel.DataAnnotations;

namespace Core.Domain;

public class Coin
{
    [Key]
    public int Denomination { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public bool IsRefillable { get; set; }
}