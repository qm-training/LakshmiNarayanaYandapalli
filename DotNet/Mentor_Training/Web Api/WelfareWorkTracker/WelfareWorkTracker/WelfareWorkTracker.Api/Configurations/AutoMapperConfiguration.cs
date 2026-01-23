namespace WelfareWorkTracker.Api.Configurations;
public static class AutoMapperConfiguration
{
    public static IMapper InitializeMapper()
    {
        var mapper = new MapperConfiguration(cnfg =>
        {
            cnfg.CreateMap<User, UserDto>();
            cnfg.CreateMap<Role, RoleDto>();
            cnfg.CreateMap<Constituency, ConstituencyDto>();
            cnfg.CreateMap<Complaint, ComplaintDto>();
            cnfg.CreateMap<ComplaintImage, ComplaintImageDto>();
            cnfg.CreateMap<ComplaintStatus, ComplaintStatusDto>();
            cnfg.CreateMap<Comment, CommentDto>();
            cnfg.CreateMap<DailyComplaint, DailyComplaintDto>();
            cnfg.CreateMap<DailyComplaintStatus, DailyComplaintStatusDto>();
            cnfg.CreateMap<FeedbackDto, FeedbackDto>();
        });
        return mapper.CreateMapper();
    }
}