using Microsoft.AspNetCore.Mvc;

namespace ChartControls
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index() => View();
    }
}