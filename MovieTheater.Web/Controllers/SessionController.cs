using Microsoft.AspNetCore.Mvc;

namespace MovieTheater.Web.Controllers
{
    public class SessionController : Controller
    {
        [Route("Session/{id}")]
        public IActionResult Session(int id)
        {
            ViewData["SessionId"] = id;
            return View("Session");
        }
    }
}