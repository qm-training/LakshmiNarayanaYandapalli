using AutoMapper;
using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Api.Configurations
{
    public static class AutoMapperConfiguration
    {
        public static IMapper InitializeMapper()
        {
            var mapper = new MapperConfiguration(cnfg =>
            {
                cnfg.CreateMap<User, UserDto>();
                cnfg.CreateMap<Role, RoleDto>();
                cnfg.CreateMap<Constituency, ConstituencyDto>();
                cnfg.CreateMap<ComplaintImage, ComplaintImageDto>();
                cnfg.CreateMap<ComplaintStatus, ComplaintStatusDto>();
                cnfg.CreateMap<Complaint, ComplaintDto>();
                cnfg.CreateMap<DailyComplaint, DailyComplaintDto>();
                cnfg.CreateMap<DailyComplaintStatus, DailyComplaintStatusDto>();
            });
                return mapper.CreateMapper();
        }
    }
}
