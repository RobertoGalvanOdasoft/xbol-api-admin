using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController(CategoryService categoryService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a collection of category names.
        /// </summary>
        /// <remarks>Use this method to obtain a list of category names for display in user interfaces or
        /// for reporting purposes. The returned collection does not include additional category details beyond the
        /// name.</remarks>
        /// <returns>An list of strings containing the names of all available categories. The collection
        /// is empty if no categories exist.</returns>
        [HttpGet("names")]
        [EndpointName("GetCategoriesNamesAsync")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public ActionResult<List<string>> GetCategoriesNamesAsync()
        {
            var result = categoryService.GetCategoryNames();
            return Ok(result);
        }
    }
}
