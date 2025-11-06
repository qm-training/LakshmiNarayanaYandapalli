using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareWorkTracker.Core.Constants
{
    public static class Constants
    {
        public static class EmailTemplates
        {
            public static string UnderValidation { get; } = "UnderValidation";
            public static string CitizenValid { get; } = "CitizenValid";
            public static string AdminValid { get; } = "AdminValid";
            public static string AdminInvalid { get; } = "AdminInvalid";
            public static string LeaderBacklog { get; } = "LeaderBacklog";
            public static string CitizenBacklog { get; } = "CitizenBacklog";
            public static string CitizenRejected { get; } = "CitizenRejected";
            public static string CitizenApprovedByLeader { get; } = "CitizenApprovedByLeader";
            public static string CitizenResolved { get; } = "CitizenResolved";
            public static string ConstituencyResolved { get; } = "ConstituencyResolved";
            public static string NewCitizenComplaintExists { get; } = "NewCitizenComplaintExists";
            public static string ExistingCitizenComplaintReopened { get; } = "ExistingCitizenComplaintReopened";
            public static string LeaderApproval { get; } = "LeaderApproval";
            public static string CitizenClosed { get; } = "CitizenClosed";
            public static string LeaderClosed { get; } = "LeaderClosed";
            public static string CitizenReopened { get; } = "CitizenReopened";
            public static string LeaderReopened { get; } = "LeaderReopened";
            public static string CitizenUnresolved { get; } = "CitizenUnresolved";
            public static string LeaderUnresolved { get; } = "LeaderUnresolved";
            public static string LeaderReputationIncreased { get; } = "LeaderReputationIncreased";
            public static string LeaderReputationDecreased { get; } = "LeaderReputationDecreased";
            public static string LeaderNewReopenAttempt { get; } = "LeaderNewReopenAttempt";
        }
    }
}
