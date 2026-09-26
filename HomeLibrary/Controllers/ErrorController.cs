using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Controllers;

public sealed class ErrorController : Controller
{
    [Route("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
