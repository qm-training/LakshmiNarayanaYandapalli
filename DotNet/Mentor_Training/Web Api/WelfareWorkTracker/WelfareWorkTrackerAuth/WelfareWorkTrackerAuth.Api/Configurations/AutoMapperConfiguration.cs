namespace WelfareWorkTrackerAuth.Api.Configurations;
public static class AutoMapperConfiguration
{
    public static IMapper InitializeMapper()
    {
        var mapper = new MapperConfiguration(cnfg =>
        {
            cnfg.CreateMap<Role, RoleDto>();
        });
        return mapper.CreateMapper();
    }

}