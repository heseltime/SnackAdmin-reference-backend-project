using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnackAdmin.BusinessLogic;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;
using SnackAdmin.Dtos;
using SnackAdmin.StatusInfo;

namespace SnackAdmin.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class BusinessRegistrationController : Controller
    {
        private readonly IBusinessRegistrationManagementLogic _logic;
        private readonly IMapper _mapper;

        public BusinessRegistrationController(IBusinessRegistrationManagementLogic logic, IMapper mapper)
        {
            _logic = logic;
            _mapper = mapper;
        }

        // POST method to register as a business (i.e. restaurant)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<RestaurantForCreationDto>> RegisterBusiness([FromBody] RestaurantForCreation restaurantForCreation)
        {
            if (restaurantForCreation == null)
            {
                return BadRequest();
            }

            // Add Restaurant to Database
            var (status, addedRestaurant) = await _logic.AddRestaurantAsync(restaurantForCreation);

            switch (status)
            {
                case 0: // Success
                    return Ok(addedRestaurant.ApiKey);

                case -1: // Conflict
                    return Conflict("A restaurant with similar data already exists.");

                case -2: // Bad Request
                    return StatusCode(StatusCodes.Status400BadRequest, "Bad Request. Please check your data transmitted.");

                case -3: // System Error
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service unavailable. Please try again later.");

                default: // Unknown Error
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
