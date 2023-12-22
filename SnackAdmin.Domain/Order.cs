using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class Order : IEntity
    {
        public Order()
        {
            
        }

        public Order(
            Guid id, int restaurantId, int addressId, int orderedBy, DateTime timestamp,
            double gpsLat, double gpsLong,
            string freeText, DeliveryStatus status)
        {
            Id = id;
            RestaurantId = restaurantId;
            AddressId = addressId;
            OrderedBy = orderedBy;
            Timestamp = timestamp;
            GpsLat = gpsLat;
            GpsLong = gpsLong;
            FreeText = freeText;
            Status = status;
        }

        public Guid Id { get; set; }
        public int RestaurantId { get; set; }
        public int AddressId { get; set; }
        public int OrderedBy { get; set; }
        public DateTime Timestamp { get; set; }
        public double GpsLat { get; set; }
        public double GpsLong { get; set; }
        public string FreeText { get; set; }
        public DeliveryStatus Status { get; set; }



        public override string ToString() => 
            $"[Order ({Id}): {RestaurantId}, {AddressId}, {OrderedBy}, {Timestamp}, {GpsLat}, {GpsLong}, {FreeText}, {Status}]";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Order otherOrder = (Order)obj;

            return Id == otherOrder.Id &&
                   RestaurantId == otherOrder.RestaurantId &&
                   AddressId == otherOrder.AddressId &&
                   OrderedBy == otherOrder.OrderedBy &&
                   GpsLat == otherOrder.GpsLat &&
                   GpsLong == otherOrder.GpsLong &&
                   FreeText == otherOrder.FreeText &&
                   Status == otherOrder.Status;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                RestaurantId,
                AddressId,
                OrderedBy,
                GpsLat,
                GpsLong,
                FreeText,
                Status
            );
        }

    }

    public enum DeliveryStatus
    {
        Unkown = 0,
        OrderPlaced = 1,
        InTheKitchen = 2,
        OnTheWay = 3,
        Delivered = 4
    }

}
