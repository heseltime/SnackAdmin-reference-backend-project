

namespace SnackAdmin.Dtos
{
    public class AddressDto
    {
        public int Id { get; set; }
        public required string Street { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Country { get; set; }
    }
}



//public int Id { get; set; }
//public string Street { get; set; }
//public string PostalCode { get; set; }
//public string City { get; set; }
//public string State { get; set; }
//public string Country { get; set; }
