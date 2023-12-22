using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

using SnackAdmin.BusinessLogic.Interfaces;
using SnackAdmin.Dtos;

namespace SnackAdmin.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuManagementLogic _menuManagementLogic;
        private readonly IMapper _mapper;

        public MenuController(IMenuManagementLogic menuManagementLogic, IMapper mapper)
        {
            _menuManagementLogic = menuManagementLogic;
            _mapper = mapper;
        }

        [HttpGet("{restaurantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<MenuDto>>> GetMenus([FromRoute] int restaurantId)
        {
            var menus = await _menuManagementLogic.GetMenusAsync(restaurantId);
            if (!menus.Any())
                return BadRequest(
                    new ProblemDetails
                    {
                        Title = "Not found",
                        Detail = $"Menus for restaurant with id {restaurantId} not found"
                    });

            var menuDtos = new List<MenuDto>();
            foreach (var menu in menus)
            {
                menuDtos.Add(_mapper.Map<MenuDto>(menu));
            }

            return Ok(menuDtos);
        }

    }
}
