using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SnackAdmin.Domain;

namespace SnackAdmin.BusinessLogic.Common
{
    public static class Calculations
    {

        public static int DistanceBetweenGpsCoordinates(double gpsLat1, double gpsLong1, double gpsLat2, double gpsLong2)
        {
            var earthRadiusKm = 6371;

            var dLat = DegreesToRadians(gpsLat2 - gpsLat1);
            var dLon = DegreesToRadians(gpsLong2 - gpsLong1);

            gpsLat1 = DegreesToRadians(gpsLat1);
            gpsLat2 = DegreesToRadians(gpsLat2);

            var a = Math.Sin(dLat / 2.0) * Math.Sin(dLat / 2.0) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(gpsLat1) * Math.Cos(gpsLat2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return (int)Math.Floor(earthRadiusKm * c);
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static decimal CalculateTotalCosts(IEnumerable<Menu> menus, IEnumerable<OrderItem> items)
        {
            var orderedMenus =
                from m in menus
                join oi in items on m.Id equals oi.MenuId
                select new { m.Price, oi.Quantity };

            decimal totalCosts = 0;
            foreach (var item in orderedMenus)
            {
                totalCosts += (item.Price * item.Quantity);
            }
            return totalCosts;
        }
    }
}
