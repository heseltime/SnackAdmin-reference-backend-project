

using SnackAdmin.Domain;

namespace SnackAdmin.Dtos
{
    public class OrderDtoForOverview
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public DeliveryStatus Status { get; set; }
    }
}
