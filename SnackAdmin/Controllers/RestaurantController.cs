using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

using SnackAdmin.BusinessLogic;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dtos;
using SnackAdmin.Dal.Interface;


namespace SnackAdmin.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(WebApiConventions))]
    public class RestaurantController : ControllerBase
    {
        private const int MinRadius = 0;
        private const int MaxRadius = 10000;
        private const double MinLatitude = -90;
        private const double MaxLatitude = 90;
        private const double MinLongitude = -180;
        private const double MaxLongitude = 180;
        private readonly IRestaurantManagementLogic _logic;
        private readonly IMapper _mapper;

        public RestaurantController(
            IRestaurantManagementLogic restaurantManagementLogic,
            IMapper mapper)
        {
            this._logic = restaurantManagementLogic;
            this._mapper = mapper;
        }

        [HttpGet("{radius}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>>
            GetRestaurants(
                [FromQuery] double longitude,
                [FromQuery] double latitude,
                [FromRoute] int radius)
        {
            if(longitude < MinLongitude || longitude > MaxLongitude)
                return BadRequest(StatusInfo.RestaurantStatusInfo.InvalidLongitudeValue(longitude, MinLongitude, MaxLongitude));

            if (latitude < MinLatitude || latitude > MaxLatitude)
                return BadRequest(StatusInfo.RestaurantStatusInfo.InvalidLatitudeValue(latitude, MinLatitude, MaxLatitude));

            if (radius < MinRadius || radius > MaxRadius)
                return BadRequest(StatusInfo.RestaurantStatusInfo.InvalidRadiusValue(radius, MinRadius, MaxRadius));

            var restaurants = await _logic.GetRestaurantsAsync(latitude, longitude, radius);
            
            var restaurantDtos = new List<RestaurantDto>();


            foreach (var r in restaurants)
            {
                var rDto = _mapper.Map<RestaurantDto>(r);

                var address = await _logic.GetAddressByIdAsync(r.AddressId);
                rDto.Address = _mapper.Map<AddressDto>(address);

                var condition = await _logic.GetDeliveryConditionForDistance(latitude, longitude, r);
                rDto.DeliveryCondition = _mapper.Map<DeliveryConditionDto>(condition);

                var actOpeningHour = await _logic.GetActualOpeningHourByRestaurantIdAsync(r.Id);
                rDto.OpeningHour = _mapper.Map<OpeningHourDtoForToday>(actOpeningHour);

                restaurantDtos.Add(rDto);
            }

            return Ok(restaurantDtos);
        }

    }
}
