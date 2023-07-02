using Microsoft.AspNetCore.Mvc;
namespace IdentityPractice.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
           // TempData["SuccessMessage"] = "LogedIn   successfully!";
           
            return View();
        }
    }
}
