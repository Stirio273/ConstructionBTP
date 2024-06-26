using Microsoft.AspNetCore.Mvc;

namespace Evaluation.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}