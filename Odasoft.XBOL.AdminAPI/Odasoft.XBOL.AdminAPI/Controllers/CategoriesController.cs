using Microsoft.AspNetCore.Mvc;
using Odasoft.XBOL.Business.Services;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController(CategoryService categoryService) : ControllerBase
    {
        [HttpGet("names")]
        [EndpointName("GetCategoriesNamesAsync")]
        public ActionResult<ICollection<string>> GetCategoriesNamesAsync()
        {
            var result = categoryService.GetCategoryNames();
            return Ok(result);
        }
    }
}
