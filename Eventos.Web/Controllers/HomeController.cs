using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Eventos.Web.Models;

namespace Eventos.Web.Controllers;

public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Error()
    {
        await Task.CompletedTask;
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
