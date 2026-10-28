using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController(
    IInscripcionRepositorio inscripcionRepositorio,
    IEventoRepositorio eventoRepositorio,
    IAsistenteRepositorio asistenteRepositorio) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await inscripcionRepositorio.ListarAsync());
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Inscripcion());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventoId,AsistenteId")] Inscripcion inscripcion)
    {
        if (ModelState.IsValid)
        {
            var mensajeError = await inscripcionRepositorio.CrearAsync(inscripcion);
            if (mensajeError == null)
            {
                TempData["Exito"] = "Inscripción confirmada.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", mensajeError);
        }
        await CargarListasAsync();
        return View(inscripcion);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (!await inscripcionRepositorio.CancelarAsync(id))
        {
            return NotFound();
        }

        TempData["Exito"] = "Inscripción cancelada.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync()
    {
        var resumenesEventos = await eventoRepositorio.ListarAsync(null);
        var eventosActivos = resumenesEventos.Select(resumenEvento => resumenEvento.Evento);
        var asistentesActivos = await asistenteRepositorio.ListarAsync();

        ViewBag.Eventos = new SelectList(eventosActivos, "Id", "Titulo");
        ViewBag.Asistentes = new SelectList(asistentesActivos, "Id", "Nombres");
    }
}
