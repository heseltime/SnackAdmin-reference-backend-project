using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class DeliveryCondition : IEntity
    {
        public DeliveryCondition()
        {
            
        }

        public DeliveryCondition(
            int id, int restaurantId, double distance, decimal minOrderValue, decimal deliveryCost)
        {
            Id = id;
            RestaurantId = restaurantId;
            Distance = distance;
            MinOrderValue = minOrderValue;
            DeliveryCost = deliveryCost;
        }

        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public double Distance { get; set; }   // in km
        public decimal MinOrderValue { get; set; }
        public decimal DeliveryCost { get; set; }


        public override string ToString() => 
            $"{RestaurantId} ({Id}): {MinOrderValue}, {DeliveryCost}";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            DeliveryCondition otherDeliveryCondition = (DeliveryCondition)obj;

            return Id == otherDeliveryCondition.Id &&
                   RestaurantId == otherDeliveryCondition.RestaurantId &&
                   Distance == otherDeliveryCondition.Distance &&
                   MinOrderValue == otherDeliveryCondition.MinOrderValue &&
                   DeliveryCost == otherDeliveryCondition.DeliveryCost;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                RestaurantId,
                Distance,
                MinOrderValue,
                DeliveryCost
            );
        }

    }

}
