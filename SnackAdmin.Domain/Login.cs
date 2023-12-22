using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class Login : IEntity
    {
        public Login()
        {
            
        }

        public Login(
            string restaurantName, string apiKey)
        {
            RestaurantName = restaurantName;
            ApiKey = apiKey;
        }

        public string RestaurantName { get; set; }
        public string ApiKey { get; set; }

        public override string ToString() => 
            $"{RestaurantName} : secret ApiKey";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Login otherLogin = (Login)obj;

            return RestaurantName == otherLogin.RestaurantName &&
                   ApiKey == otherLogin.RestaurantName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                RestaurantName,
                ApiKey
            );
        }


    }

}
