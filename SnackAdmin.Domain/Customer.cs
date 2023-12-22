using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class Customer : IEntity
    {
        public Customer(int id, string userName, string passwordHash, string salt)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
            Salt = salt;
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }


        public override string ToString() => 
            $"({Id}): {UserName}, {PasswordHash}, {Salt}";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Customer otherAddress = (Customer)obj;

            return Id == otherAddress.Id &&
                   UserName == otherAddress.UserName &&
                   PasswordHash == otherAddress.PasswordHash &&
                   Salt == otherAddress.Salt;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                UserName,
                PasswordHash,
                Salt
            );
        }


    }
}
