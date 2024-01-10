

using SnackAdmin.Domain;

namespace SnackAdmin.Dtos
{
    public class MenuDto
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; } 
        public required string Category { get; set; }
        public required string ItemName { get; set; }
        public string Description { get; set; }
        public required decimal Price { get; set; }
    }
}

//public int Id { get; set; }
//public int RestaurantId { get; set; }
//public string Category { get; set; }
//public string ItemName { get; set; }
//public string Description { get; set; }
//public decimal Price { get; set; }
