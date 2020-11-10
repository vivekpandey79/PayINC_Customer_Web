using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Models
{
    public class MenuModel
    {

    }
    public class MenuRes
    {
        public int customerRoleId { get; set; }
        public int menuId { get; set; }
        public int? menuParentId { get; set; }
        public string menuParentText { get; set; }
        public string menuText { get; set; }
        public string menuUrl { get; set; }
        public int menuStatus { get; set; }
        public int menuSet { get; set; }
        public int webPortalId { get; set; }
        public string webPortalName { get; set; }
        public string webPortalUrl { get; set; }
        public int? subMenu { get; set; }
        public int menuTypeId { get; set; }
        public int webPortalStatus { get; set; }
        public string menuIcon { get; set; }

    }
}
