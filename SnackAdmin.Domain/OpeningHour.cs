using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class OpeningHour : IEntity
    {
        public OpeningHour(int id, int restaurantId, Day day, TimeSpan openTime, TimeSpan closeTime)
        {
            Id = id;
            RestaurantId = restaurantId;
            Day = day;
            OpenTime = openTime;
            CloseTime = closeTime;
        }

        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public Day Day { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }

        public override string ToString() =>
            $"[Order ({Id}): {RestaurantId}, {Day}, {OpenTime}, {CloseTime}]";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            OpeningHour otherOpeningHour = (OpeningHour)obj;

            return Id == otherOpeningHour.Id &&
                   RestaurantId == otherOpeningHour.RestaurantId &&
                   Day == otherOpeningHour.Day &&
                   OpenTime == otherOpeningHour.OpenTime &&
                   CloseTime == otherOpeningHour.CloseTime;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                RestaurantId,
                Day,
                OpenTime,
                CloseTime
            );
        }
    }

    public enum Day
    {
        Unknown = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sonday = 7,
        Holiday = 8
    }
}
