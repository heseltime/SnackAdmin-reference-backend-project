using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using SnackAdmin.BusinessLogic;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dal.Interface;
using SnackAdmin.Domain;
using SnackAdmin.Dtos;
using SnackAdmin.StatusInfo;
using SnackAdmin.Services;
using YamlDotNet.Core.Tokens;
using Azure;
using Microsoft.AspNetCore.JsonPatch;

namespace SnackAdmin.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    [ApiController]
    public class BusinessController : Controller
    {
        private readonly IBusinessManagementLogic _logic;
        private readonly IMapper _mapper;

        private readonly JwtTokenService _jwtTokenService;

        public BusinessController(IBusinessManagementLogic logic, IMapper mapper, JwtTokenService jwtTokenService)
        {
            _logic = logic;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
        }

        //
        // Orders Endpoints
        //

        // GET method to retrieve all orders
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            // get the relevant restaurant name (subject field) from the authenticated user's JWT token
            var restaurantName = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var orders = await _logic.GetAllOrdersAsync(restaurantName); 
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders); 

            if (ordersDto != null)
                return Ok(ordersDto);
            else
                return NotFound(); // either because restaurant not found by api key or no orders,
                                   // but this information is not shared
        }

        // GET method to retrieve a specific order by ID
        [HttpGet("orders/{orderId}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid orderId)
        {
            var order = await _logic.GetOrderByIdAsync(orderId);
            var orderDto = _mapper.Map<OrderDto>(order);

            if (orderDto != null)
                return Ok(orderDto);
            else
                return NotFound();
        }

        // PUT method to update an order
        [HttpPut("orders/{orderId}")]
        public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest();
            }

            var existingOrder = await _logic.GetOrderByIdAsync(orderId);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // Map the updated fields from the DTO to the existing order entity
            _mapper.Map(orderDto, existingOrder);

            // Update in the database 
            var updateResult = await _logic.UpdateOrderAsync(existingOrder);

            if (updateResult == 0)
            {
                return NoContent(); // 204 No Content typically returned when an update is successful
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the order.");
            }
        }

        // PATCH method to update an order, esp. status
        //[HttpPatch("orders/{orderId}")]
        //public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody] JsonPatchDocument<OrderDto> patchDoc)
        //{
        //    if (patchDoc == null)
        //    {
        //        return BadRequest();
        //    }

        //    var existingOrder = await _logic.GetOrderByIdAsync(orderId);
        //    if (existingOrder == null)
        //    {
        //        return NotFound();
        //    }

        //    // Create a DTO from the existing order entity
        //    OrderDto orderDto = _mapper.Map<OrderDto>(existingOrder);

        //    // Apply the patch to the DTO
        //    patchDoc.ApplyTo(orderDto, ModelState);

        //    // Validate the changes
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Map the updated fields from the DTO back to the existing order entity
        //    //_mapper.Map(orderDto, existingOrder);
        //    _mapper.Map(orderDto, existingOrder);

        //    // Update in the database 
        //    var updateResult = await _logic.UpdateOrderAsync(existingOrder);

        //    if (updateResult == 0)
        //    {
        //        return NoContent(); // 204 No Content typically returned when an update is successful
        //    }
        //    else
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the order.");
        //    }
        //}

        // POST just for status
        [HttpPost("orderStatusUpdate/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] int newStatus)
        {
            if (newStatus == null || newStatus < 0 || newStatus > 4)
            {
                return BadRequest();
            }

            var existingOrder = await _logic.GetOrderByIdAsync(orderId);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // change status
            existingOrder.Status = (DeliveryStatus)newStatus;

            // Update in the database 
            var updateResult = await _logic.UpdateOrderAsync(existingOrder);

            if (updateResult == 0)
            {
                return NoContent(); // 204 No Content typically returned when an update is successful
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the order.");
            }
        }


        // PUT method to update an order - With Token!
        [HttpPut("orderLink/{orderToken}")]
        public async Task<IActionResult> UpdateOrderByToken(string orderToken, [FromQuery] int status)
        {
            // validate status parameter
            if (status < 0 || status > 4)
            {
                return BadRequest();
            }

            Order existingOrder = await _logic.GetOrderByTokenAsync(orderToken);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // Only Update Status on the existing order
            existingOrder.Status = (DeliveryStatus)status;

            // and encode into a new token (free text)
            string newToken = _jwtTokenService.GenerateTokenWithStatus(existingOrder);
            existingOrder.FreeText = newToken;

            // Update in the database 
            var updateResult = await _logic.UpdateOrderAsync(existingOrder);

            if (updateResult == 0)
            {
                //return NoContent(); // 204 No Content typically returned when an update is successful
                return Ok(new { newToken });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the order.");
            }
        }


        //
        // Menu Endpoints
        //

        // GET method to retrieve all menus for a specific restaurant
        [HttpGet("menu")]
        public async Task<ActionResult<IEnumerable<MenuDto>>> GetMenu()
        {
            // get the relevant restaurant name (subject field) from the authenticated user's JWT token
            var restaurantName = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var menuItems = await _logic.GetAllMenuItemsAsync(restaurantName);
            var menuItemsDto = _mapper.Map<IEnumerable<MenuDto>>(menuItems);

            if (menuItemsDto != null)
                return Ok(menuItemsDto);
            else
                return NotFound(); // either because restaurant not found by api key or no orders,
                                   // but this information is not shared
        }

        // PUT method to update a menu
        [HttpPut("menu/{menuId}")]
        public async Task<IActionResult> UpdateMenu(int menuId, [FromBody] MenuDto menuDto)
        {
            if (menuDto == null)
            {
                return BadRequest();
            }

            var existingMenu = await _logic.GetMenuByIdAsync(menuId);
            if (existingMenu == null)
            {
                return NotFound();
            }

            // Map the updated fields from the DTO to the existing menu entity
            _mapper.Map(menuDto, existingMenu);

            // Update in the database 
            var updateResult = await _logic.UpdateMenuAsync(existingMenu);

            if (updateResult == 0)
            {
                return NoContent(); // 204 No Content typically returned when an update is successful
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the order.");
            }
        }

        // POST method to add a new menu
        [HttpPost("menu")]
        public async Task<IActionResult> AddMenu([FromBody] MenuDto menuDto)
        {
            if (menuDto == null)
            {
                return BadRequest();
            }

            // Create a new menu entity from the DTO
            var newMenu = _mapper.Map<Menu>(menuDto);

            // Add to the database
            var addResult = await _logic.AddMenuAsync(newMenu);

            if (addResult == 0)
            {
                return CreatedAtAction(nameof(GetMenu), new { menuId = newMenu.Id }, newMenu); // id not correct
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the menu.");
            }
        }


        // DELETE method to delete a delivery condition
        [HttpDelete("menu/{menuId}")]
        public async Task<IActionResult> DeleteMenu(int menuId)
        {
            var existingMenu = await _logic.GetMenuByIdAsync(menuId);
            if (existingMenu == null)
            {
                return NotFound();
            }

            // check the authenticated restaurant is requestion own data:
            var restaurantName = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; // authenticated restaurant by name
            var restaurant = await _logic.GetRestaurantByname(restaurantName);

            if (restaurant == null || restaurant.Id != existingMenu.RestaurantId)
            {
                return BadRequest();
            }


            // Delete the condition from the database
            var deleteResult = await _logic.DeleteMenuAsync(existingMenu);

            if (deleteResult == 0)
            {
                return Ok(); // 200 OK typically returned when a delete operation is successful
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the condition.");
            }
        }


        //
        // Delivery Conditions Endpoints
        //

        // GET method to retrieve all delivery conditions for a specific restaurant
        [HttpGet("conditions")]
        public async Task<ActionResult<IEnumerable<DeliveryConditionDto>>> GetConditions()
        {
            // get the relevant restaurant name (subject field) from the authenticated user's JWT token
            var restaurantName = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var deliveryConditions = await _logic.GetAllDeliveryConditionsAsync(restaurantName);
            var deliveryConditionsDto = _mapper.Map<IEnumerable<DeliveryConditionDto>>(deliveryConditions);

            if (deliveryConditionsDto != null)
                return Ok(deliveryConditionsDto);
            else
                return NotFound(); // either because restaurant not found by api key or no orders,
                                   // but this information is not shared
        }

        // PUT method to update a delivery condition
        [HttpPut("conditions/{conditionId}")]
        public async Task<IActionResult> UpdateCondition(int conditionId, [FromBody] DeliveryConditionDto conditionDto)
        {
            if (conditionDto == null)
            {
                return BadRequest();
            }

            var existingCondition = await _logic.GetDeliveryConditionByIdAsync(conditionId);
            if (existingCondition == null)
            {
                return NotFound();
            }

            // Map the updated fields from the DTO to the existing menu entity
            _mapper.Map(conditionDto, existingCondition);

            // Update in the database 
            var updateResult = await _logic.UpdateDeliveryConditionAsync(existingCondition);

            if (updateResult == 0)
            {
                return NoContent(); // 204 No Content typically returned when an update is successful
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the order.");
            }
        }

        // DELETE method to delete a delivery condition
        [HttpDelete("conditions/{conditionId}")]
        public async Task<IActionResult> DeleteCondition(int conditionId)
        {
            var existingCondition = await _logic.GetDeliveryConditionByIdAsync(conditionId);
            if (existingCondition == null)
            {
                return NotFound();
            }

            // check the authenticated restaurant is requestion own data:
            var restaurantName = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; // authenticated restaurant by name
            var restaurant = await _logic.GetRestaurantByname(restaurantName);

            if (restaurant == null || restaurant.Id != existingCondition.RestaurantId)
            {
                return BadRequest();
            }


            // Delete the condition from the database
            var deleteResult = await _logic.DeleteDeliveryConditionAsync(existingCondition);

            if (deleteResult == 0)
            {
                return Ok(); // 200 OK typically returned when a delete operation is successful
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the condition.");
            }
        }

    }
}
