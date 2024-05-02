using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Drink
{
    [Key]
    public int Key { get; set; }
    
    [Required]
    public string ImagePath { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public int Cost { get; set; }
    
    [Required]
    public int Quantity { get; set; }
}