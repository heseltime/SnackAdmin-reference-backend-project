

using SnackAdmin.Domain;

namespace SnackAdmin.Dtos
{
    public class OpeningHourDtoForToday
    {
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
    }
}

//public int Id { get; set; }
//public int RestaurantId { get; set; }
//public Day Day { get; set; }
//public TimeSpan OpenTime { get; set; }
//public TimeSpan CloseTime { get; set; }

