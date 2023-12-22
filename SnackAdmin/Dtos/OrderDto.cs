

using SnackAdmin.Domain;

namespace SnackAdmin.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public required double GpsLat { get; set; }
        public required double GpsLong { get; set; }
        public string FreeText { get; set; }
        public DeliveryStatus Status { get; set; }
        public required AddressDto Address { get; set; }
        public required RestaurantDto Restaurant { get; set; }
        public required IEnumerable<OrderItemDto> Items { get; set; }
    }
}

//public int Id { get; set; }
//public int RestaurantId { get; set; }
//public int AddressId { get; set; }
//public int OrderedBy { get; set; }
//public DateTime Timestamp { get; set; }
//public double GpsLat { get; set; }
//public double GpsLong { get; set; }
//public string FreeText { get; set; }
//public DeliveryStatus Status { get; set; }
