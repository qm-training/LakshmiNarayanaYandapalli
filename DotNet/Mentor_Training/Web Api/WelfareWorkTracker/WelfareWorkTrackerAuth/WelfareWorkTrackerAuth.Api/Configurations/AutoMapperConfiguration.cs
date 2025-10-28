using AutoMapper;

namespace WelfareTracker.Api.Configurations
{
    public static class AutoMapperConfiguration
    {
        public static IMapper InitializeMapper()
        {
            var mapper = new MapperConfiguration(cnfg =>
            {
                //cnfg.CreateMap<User, UserDto>();
            });
            return mapper.CreateMapper();
        }

    }
}