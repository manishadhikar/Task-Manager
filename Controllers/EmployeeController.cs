using Microsoft.AspNetCore.Mvc;

namespace IdentityPractice.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
           //   TempData["SuccessMessage"] = "";
            return View();
        }
    }
}
