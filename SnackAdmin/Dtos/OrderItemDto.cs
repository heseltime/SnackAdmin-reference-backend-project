

using SnackAdmin.Domain;

namespace SnackAdmin.Dtos
{
    public class OrderItemDto
    {
        public required MenuDto Menu { get; set; }
        public required int Quantity { get; set; }
    }
}


//public int OrderId { get; set; }
//public int MenuId { get; set; }
//public int Quantity { get; set; }

