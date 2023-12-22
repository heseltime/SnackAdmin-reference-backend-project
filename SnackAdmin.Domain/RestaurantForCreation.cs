using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class RestaurantForCreation : IEntity
    {
        public RestaurantForCreation()
        {
            
        }

        public RestaurantForCreation(
            int id, string name, Address address, double gpsLat, double gpsLong,
            string webHookUrl, byte[] titleImage, OpeningHour[] openingHours)
        {
            Id = id;
            Name = name;
            NewAddress = address;
            GpsLat = gpsLat;
            GpsLong = gpsLong;
            WebHookUrl = webHookUrl;
            TitleImage = titleImage;
            OpeningHours = openingHours;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Address NewAddress { get; set; }
        public double GpsLat { get; set; }
        public double GpsLong { get; set; }
        public string WebHookUrl { get; set; }
        public byte[]? TitleImage { get; set; }
        public OpeningHour[] OpeningHours { get; set; }


        public override string ToString() => 
            $"[RestaurantForCreation {Name} ({Id}): {NewAddress}, {GpsLat}, {GpsLong}, {WebHookUrl}, {TitleImage}, {OpeningHours}]";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            RestaurantForCreation otherRestaurant = (RestaurantForCreation)obj;

            return Id == otherRestaurant.Id &&
                   Name == otherRestaurant.Name &&
                   NewAddress == otherRestaurant.NewAddress &&
                   GpsLat == otherRestaurant.GpsLat &&
                   GpsLong == otherRestaurant.GpsLong &&
                   WebHookUrl == otherRestaurant.WebHookUrl &&
                   TitleImage == otherRestaurant.TitleImage &&
                   OpeningHours == otherRestaurant.OpeningHours;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                Name,
                NewAddress,
                GpsLat,
                GpsLong,
                WebHookUrl,
                TitleImage,
                OpeningHours
            );
        }

    }

}
