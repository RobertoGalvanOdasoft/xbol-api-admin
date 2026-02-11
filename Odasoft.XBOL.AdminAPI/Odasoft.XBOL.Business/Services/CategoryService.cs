using Odasoft.XBOL.Commons.Enums;

namespace Odasoft.XBOL.Business.Services
{
    public class CategoryService
    {
        public IList<string> GetCategoryNames()
        {
            return Enum.GetNames<EventCategory>().ToList();
        }
    }
}
