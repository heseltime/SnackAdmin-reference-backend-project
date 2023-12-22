using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnackAdmin.Domain
{
    public class Address : IEntity
    {
        public Address(int id, string street, string postalCode, string city, string state, string country)
        {
            Id = id;
            Street = street;
            PostalCode = postalCode;
            City = city;
            State = state;
            Country = country;
        }

        public int Id { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }


        public override string ToString() => 
            $"({Id}): {Street}, {PostalCode}, {City}, {State}, {Country}";

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Address otherAddress = (Address)obj;

            return Id == otherAddress.Id &&
                   Street == otherAddress.Street &&
                   PostalCode == otherAddress.PostalCode &&
                   City == otherAddress.City &&
                   State == otherAddress.State &&
                   Country == otherAddress.Country;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                Street,
                PostalCode,
                City,
                State,
                Country
            );
        }


    }
}
