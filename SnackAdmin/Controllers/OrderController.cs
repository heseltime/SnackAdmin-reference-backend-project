using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SnackAdmin.BusinessLogic;
using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dtos;
using SnackAdmin.StatusInfo;

using SnackAdmin.Domain;
using MySqlX.XDevAPI.Common;
using SnackAdmin.Services;

namespace SnackAdmin.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(WebApiConventions))]
    public class OrderController : ControllerBase
    {
        private readonly IOrderManagementLogic _logic;
        private readonly IMapper _mapper;

        private readonly JwtTokenService _jwtTokenService;

        private readonly WebHookController _webHookController;

        public OrderController(IOrderManagementLogic logic, IMapper mapper, WebHookController webHookController, JwtTokenService jwtTokenService)
        {
            _logic = logic;
            _mapper = mapper;
            _webHookController = webHookController;
            _jwtTokenService = jwtTokenService;
        }


        [HttpGet("{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> GetOrder([FromRoute] Guid orderId)
        {
            // order dto
            var order = await _logic.GetOrderAsync(orderId);

            if (order == null)
                return BadRequest(OrderStatusInfo.OrderNotFound(orderId));

            var orderDto = _mapper.Map<OrderDto>(order);
            var address = await _logic.GetAddressAsync(order.AddressId);
            orderDto.Address = _mapper.Map<AddressDto>(address);

            // restaurant dto
            var restaurantDto = await CreateRestaurantDto(order);

            if (restaurantDto == null)
                return BadRequest(OrderStatusInfo.RestaurantNotFound(order.RestaurantId));

            orderDto.Restaurant = restaurantDto;
            
            // ordered items
            var items = await _logic.GetOrderItemsAsync(orderId);
            var orderItemDtos = new List<OrderItemDto>();

            foreach (var item in items)
            {
                var orderItemDto = _mapper.Map<OrderItemDto>(item);
                var menu = await _logic.GetMenuByIdAsync(item.MenuId);
                orderItemDto.Menu = _mapper.Map<MenuDto>(menu);
                orderItemDtos.Add(orderItemDto);
            }
            orderDto.Items = orderItemDtos;

            return Ok(orderDto);
        }

        private async Task<RestaurantDto?> CreateRestaurantDto(Order order)
        {
            var restaurant = await _logic.GetRestaurantAsync(order.RestaurantId);
            if (restaurant == null)
                return null;

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

            var address = await _logic.GetAddressAsync(restaurant.AddressId);
            var addressDto = _mapper.Map<AddressDto>(address);

            var deliveryCondition = await _logic.GetDeliveryConditionAsync(restaurant, order);
            var deliveryConditionDto = _mapper.Map<DeliveryConditionDto>(deliveryCondition);

            restaurantDto.Address = addressDto;
            restaurantDto.DeliveryCondition = deliveryConditionDto;

            return restaurantDto;
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<OrderDto>>
            CreateOrder(
                [FromBody] OrderDto orderDto)
        {
            //if (orderDto is null)
            //    return BadRequest();

            //if (orderDto.Address is null)
            //    return BadRequest(OrderStatusInfo.AddressIsNull());

            //if (orderDto.Restaurant is null)
            //    return BadRequest(OrderStatusInfo.RestaurantIsNull());

            //if (orderDto.Items is null)
            //    return BadRequest(OrderStatusInfo.ItemListIsNull());

            if (!orderDto.Items.Any())
                return BadRequest(OrderStatusInfo.ItemListIsEmpty());

            var order = _mapper.Map<Order>(orderDto);
            var orderItems = new List<OrderItem>();
            foreach (var item in orderDto.Items)
                orderItems.Add(_mapper.Map<OrderItem>(item));

            var address = _mapper.Map<Address>(orderDto.Address);

            // for Requirment 11: we will add a token as free text
            string token = _jwtTokenService.GenerateTokenWithStatus(order); // encodes order id and status essentially


            // in case of success, return value is order id from db
            var creationResult = await _logic.AddOrderAsync(order, orderItems, address, token);

            switch (creationResult.Item1)
            {
                case (int)OrderManagementLogic.AddOrderFailureCode.Default:
                    return Conflict(OrderStatusInfo.DefaultCreationFailure());
                    break;
                case (int)OrderManagementLogic.AddOrderFailureCode.RestaurantNotFound:
                    return BadRequest(OrderStatusInfo.RestaurantNotFound(orderDto.Restaurant.Id));
                    break;
                case (int)OrderManagementLogic.AddOrderFailureCode.RestaurantHasNotOpened:
                    return Conflict(OrderStatusInfo.RestaurantHasNotOpened(orderDto.Restaurant.Id));
                    break;
                case (int)OrderManagementLogic.AddOrderFailureCode.OneOrMoreItemsNotFound:
                    return BadRequest(OrderStatusInfo.OneOrMoreItemsNotFound());
                    break;
                case (int)OrderManagementLogic.AddOrderFailureCode.MatchingDeliveryConditionNotFound:
                    return BadRequest(OrderStatusInfo.MatchingDeliveryConditionNotFound());
                    break;
                case (int)OrderManagementLogic.AddOrderFailureCode.TotalCostsBelowMinOrderValue:
                    return Conflict(OrderStatusInfo.TotalCostsBelowMinOrderValue());
                    break;
                default:
                    orderDto.Id = creationResult.Item2;

                    // webHook
                    var restaurant = await _logic.GetRestaurantAsync(orderDto.Restaurant.Id);
                    var webHookResult =
                        await _webHookController.SendOrderToRestaurantWebHook(restaurant?.WebHookUrl, orderDto);

                    if (webHookResult is OkResult okResult)
                    {
                        return CreatedAtAction(
                            actionName: nameof(CreateOrder),
                            routeValues: new { orderId = orderDto.Id },
                            value: orderDto.Id);
                    }
                    else 
                    {
                        await _logic.DeleteOrderAsync(creationResult.Item2);
                        return  StatusCode(
                            (webHookResult as ObjectResult)?.StatusCode ?? 503, 
                            (webHookResult as ObjectResult)?.Value
                            );
                    }
                
            }
        }
    }
}
