using AutoMapper;
using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

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
                cnfg.CreateMap<ComplaintFeedback, ComplaintFeedbackDto>();

                cnfg.CreateMap<CreateNotificationVm, Notification>();
                cnfg.CreateMap<CreateEmailTemplateVm, EmailTemplate>();
                cnfg.CreateMap<UpdateEmailTemplateVm, EmailTemplate>();
            });
                return mapper.CreateMapper();
        }
    }
}
