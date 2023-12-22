

using SnackAdmin.Domain;

namespace SnackAdmin.Dtos
{
    public class DeliveryConditionDto
    {
        public int Id { get; set; }
        //public int RestaurantId { get; set; }
        public double Distance { get; set; }   // in km
        public decimal MinOrderValue { get; set; }
        public decimal DeliveryCost { get; set; }
    }
}

//public int Id { get; set; }
//public int RestaurantId { get; set; }
//public double Distance { get; set; }   // in km
//public decimal MinOrderValue { get; set; }
//public decimal DeliveryCost { get; set; }
