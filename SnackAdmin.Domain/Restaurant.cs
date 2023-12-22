using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class Restaurant : IEntity
    {
        public Restaurant()
        {
            
        }

        public Restaurant(
            int id, string name, int addressId, double gpsLat, double gpsLong,
            string webHookUrl, byte[] titleImage, string apiKey)
        {
            Id = id;
            Name = name;
            AddressId = addressId;
            GpsLat = gpsLat;
            GpsLong = gpsLong;
            WebHookUrl = webHookUrl;
            TitleImage = titleImage;
            ApiKey = apiKey;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
        public double GpsLat { get; set; }
        public double GpsLong { get; set; }
        public string WebHookUrl { get; set; }
        public byte[]? TitleImage { get; set; }
        public string ApiKey { get; set; }


        public override string ToString() => 
            $"[Restaurant {Name} ({Id}): {AddressId}, {GpsLat}, {GpsLong}, {WebHookUrl}, {TitleImage}, {ApiKey}]";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Restaurant otherRestaurant = (Restaurant)obj;

            return Id == otherRestaurant.Id &&
                   Name == otherRestaurant.Name &&
                   AddressId == otherRestaurant.AddressId &&
                   GpsLat == otherRestaurant.GpsLat &&
                   GpsLong == otherRestaurant.GpsLong &&
                   WebHookUrl == otherRestaurant.WebHookUrl &&
                   TitleImage == otherRestaurant.TitleImage &&
                   ApiKey == otherRestaurant.ApiKey;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                Name,
                AddressId,
                GpsLat,
                GpsLong,
                WebHookUrl,
                TitleImage,
                ApiKey
            );
        }

    }

}
