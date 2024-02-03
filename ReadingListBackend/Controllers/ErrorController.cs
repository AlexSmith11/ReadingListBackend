using Microsoft.AspNetCore.Mvc;

namespace ReadingListBackend.Controllers
{
    [Route("/error")]
    public class ErrorController : Controller
    {
        [Route("{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;

            // You can add more logic here to customize the error message
            // e.g. based on the status code
            return View();
        }
    }
}