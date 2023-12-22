
using SnackAdmin.Domain;

namespace SnackAdmin.Dtos
{
    public class RestaurantForCreationDto
    { 
        public int Id { get; set; }
        public required string Name { get; set; }
        public AddressDto? Address { get; set; }
        public required double GpsLat { get; set; }
        public required double GpsLong { get; set; }
        public string? WebHookUrl { get; set; }
        public byte[]? TitleImage { get; set; }
    }
}
