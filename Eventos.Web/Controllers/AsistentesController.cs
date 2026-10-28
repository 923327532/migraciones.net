using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Controllers;

public class AsistentesController(IAsistenteRepositorio asistenteRepositorio) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await asistenteRepositorio.ListarAsync());
    }

    public async Task<IActionResult> Create()
    {
        await Task.CompletedTask;
        return View(new Asistente());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombres,Dni,Correo,Telefono")] Asistente asistente)
    {
        asistente.Correo = asistente.Correo?.Trim() ?? "";
        if (await asistenteRepositorio.DniExisteAsync(asistente.Dni))
        {
            ModelState.AddModelError(nameof(asistente.Dni), "El DNI ya está registrado.");
        }

        if (await asistenteRepositorio.CorreoExisteAsync(asistente.Correo))
        {
            ModelState.AddModelError(nameof(asistente.Correo), "El correo ya está registrado.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                await asistenteRepositorio.CrearAsync(asistente);
                TempData["Exito"] = "Asistente registrado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException excepcion) when (excepcion.InnerException is Microsoft.Data.SqlClient.SqlException
            {
                Number: 2601 or 2627
            })
            {
                ModelState.AddModelError("", "El DNI o correo ya está registrado.");
            }
        }
        return View(asistente);
    }
}
