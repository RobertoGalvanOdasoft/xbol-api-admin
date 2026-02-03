using EntityDTO = Odasoft.XBOL.DTO.RoleDTO;
using EntityModel = Odasoft.XBOL.Models.Role;

namespace Odasoft.XBOL.Models.Mappers
{
    public static class RoleMapper
    {
        public static List<EntityDTO> ToDto(this IList<EntityModel> entities) => entities.Select(x => x.ToDto()).ToList();

        public static EntityDTO ToDto(this EntityModel entity)
        {
            return new EntityDTO
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty
            };
        }

        public static List<EntityModel> ToModel(this IList<EntityDTO> entities) => entities.Select(x => x.ToModel()).ToList();

        public static EntityModel ToModel(this EntityDTO entity)
        {
            return new EntityModel
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }
    }
}
