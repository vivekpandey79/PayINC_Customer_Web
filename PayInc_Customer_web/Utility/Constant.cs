using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Utility
{
    public static class APIMethodConst
    {
        public const string GetCustomerProfile = "geCustomerProfile";

        public const string GetMenusPortalRoleId = "getMenusPortalRoleId";
        public const string GetLoginAuth = "getLoginAuth";
        public const string GetBalanceByCustomerId = "getBalanceByCustomerId";
        public const string GetServiceProvidersByTypeId = "getServiceProvidersByTypeId";
        public const string ChangePassword = "getChangePassword";
        public const string ResendPassword = "getResendPassword";
    }
    public static class PortalDetails
    {
        public const int PortalId=2;
    }
}
