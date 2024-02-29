using Microsoft.AspNetCore.Mvc;

namespace ReadingListBackend.Controllers
{
    [ApiController]
    [Route("/error")]
    public class ErrorController : Controller
    {
        [Route("{statusCode}")]
        protected IActionResult HandleError(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;

            // You can add more logic here to customize the error message
            // e.g. based on the status code
            return View();
        }
    }
}