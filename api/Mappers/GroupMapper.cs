using api.Entities;
using api.Models.CampusGroup;

namespace api.Mappers
{
    public abstract class GroupMapper
    {
        public static CampusGroup MapFromCreateCampusGroupModelToCampusGroup(CreateCampusGroupModel createCampusGroupModel)
        {
            return new CampusGroup
            {
                Id = Guid.NewGuid(),
                Name = createCampusGroupModel.Name
            };
        }

        public static CampusGroupModel MapFromCampusGroupToCampusGroupModel(CampusGroup campusGroup)
        {
            return new CampusGroupModel
            {
                Id = campusGroup.Id,
                Name = campusGroup.Name
            };
        }
    }
}
