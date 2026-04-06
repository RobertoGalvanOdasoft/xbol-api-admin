using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;
using Odasoft.XBOL.DTO.Results;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController(CategoryService categoryService) : ControllerBase
    {
        /// <summary>
        /// Retrieves the list of active event categories.
        /// </summary>
        /// <returns>A list of event categories with their id, name, and display name.</returns>
        [HttpGet]
        [EndpointName("GetCategoriesAsync")]
        [ProducesResponseType(typeof(List<EventCategoryResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EventCategoryResult>>> GetCategoriesAsync()
        {
            var result = await categoryService.GetCategoriesAsync();
            return Ok(result);
        }
    }
}
