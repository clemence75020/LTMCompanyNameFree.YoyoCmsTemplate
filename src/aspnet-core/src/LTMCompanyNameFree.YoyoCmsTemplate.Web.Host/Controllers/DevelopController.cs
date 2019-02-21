using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Abp;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Timing;
using LTMCompanyNameFree.YoyoCmsTemplate.Controllers;

namespace LTMCompanyNameFree.YoyoCmsTemplate.Web.Host.Controllers
{
    public class DevelopController : YoyoCmsTemplateControllerBase
    {
        private readonly INotificationPublisher _notificationPublisher;

        public DevelopController(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public IActionResult Index()
        {
#if DEBUG
            return Redirect("/swagger");
#endif

#if !DEBUG
            return Redirect("/public");
#endif

        }
    }
}
