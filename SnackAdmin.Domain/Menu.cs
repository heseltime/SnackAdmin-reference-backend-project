using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class Menu : IEntity
    {
        public Menu()
        {
            
        }

        public Menu(
            int id, int restaurantId, string category, string itemName,
            string description, decimal price)
        {
            Id = id;
            RestaurantId = restaurantId;
            Category = category;
            ItemName = itemName;
            Description = description;
            Price = price;
        }

        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string Category { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }


        public override string ToString() => 
            $"{RestaurantId} ({Id}): {Category}, {ItemName}, {Description}, {Price}";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Menu otherMenu = (Menu)obj;

            return Id == otherMenu.Id &&
                   RestaurantId == otherMenu.RestaurantId &&
                   Category == otherMenu.Category &&
                   ItemName == otherMenu.ItemName &&
                   Description == otherMenu.Description &&
                   Price == otherMenu.Price;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                RestaurantId,
                Category,
                ItemName,
                Description,
                Price
            );
        }


    }

}
