using Grand.Framework.Components;
using Grand.Services.Security;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Grand.Web.Areas.Admin.Components
{
    public class CustomerReportCustomerTimeChartViewComponent : BaseAdminViewComponent
    {
        private readonly IPermissionService _permissionService;

        public CustomerReportCustomerTimeChartViewComponent(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageReports))
                return Content("");

            return View();
        }
    }
}