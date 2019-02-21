using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Abp;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Timing;
using LTMCompanyNameFree.YoyoCmsTemplate.Controllers;

namespace LTMCompanyNameFree.YoyoCmsTemplate.Web.Host.Controllers
{
    public class HomeController : YoyoCmsTemplateControllerBase
    {
        public HomeController()
        {

        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
            //return RedirectToAction("Api");
            //return Redirect("/api");
        }
    }
}
