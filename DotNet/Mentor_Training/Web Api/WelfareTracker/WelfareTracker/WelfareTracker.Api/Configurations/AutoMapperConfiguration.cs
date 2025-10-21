using AutoMapper;
using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Models;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Api.Configurations
{
    public static class AutoMapperConfiguration
    {
        public static IMapper InitializeMapper()
        {
            var mapper = new MapperConfiguration(cnfg =>
            {
                cnfg.CreateMap<User, UserDto>();

                cnfg.CreateMap<UserVm, User>()
                    .ForMember(dest => dest.ConstituencyName, opt => opt.MapFrom(src => src.ConstituencyName))
                    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleName))
                    .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                    .ForMember(dest => dest.ConstituencyId, opt => opt.Ignore())
                    .ForMember(dest => dest.DateCreated, opt => opt.Ignore())
                    .ForMember(dest => dest.DateUpdated, opt => opt.Ignore())
                    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                    .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                    .ForMember(dest => dest.Reputation, opt => opt.Ignore());

                cnfg.CreateMap<Complaint, ComplaintDto>()
                    .ForMember(dest => dest.Status, opt => opt.Ignore());

                cnfg.CreateMap<Comment, CommentDto>();
            });
            return mapper.CreateMapper();
        }

    }
}
