using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController(IEventoRepositorio eventoRepositorio) : Controller
{
    public async Task<IActionResult> Index(int? categoriaId)
    {
        await CargarCategoriasAsync(categoriaId);
        return View(await eventoRepositorio.ListarAsync(categoriaId));
    }

    public async Task<IActionResult> Details(int id)
    {
        var resumenEvento = await eventoRepositorio.ObtenerAsync(id);
        return resumenEvento == null ? NotFound() : View(resumenEvento);
    }

    public async Task<IActionResult> Create()
    {
        await CargarCategoriasAsync();
        var evento = new Evento
        {
            Fecha = DateTime.Now.AddDays(1),
            Aforo = 50
        };

        return View(evento);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        await ValidarCategoriaAsync(evento);
        if (ModelState.IsValid)
        {
            await eventoRepositorio.CrearAsync(evento);
            return RedirigirConExito("Evento creado.");
        }
        await CargarCategoriasAsync(evento.CategoriaId);
        return View(evento);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var resumenEvento = await eventoRepositorio.ObtenerAsync(id);
        if (resumenEvento == null)
        {
            return NotFound();
        }

        await CargarCategoriasAsync(resumenEvento.Evento.CategoriaId);
        return View(resumenEvento.Evento);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        if (id != evento.Id)
        {
            return BadRequest();
        }

        await ValidarCategoriaAsync(evento);
        if (ModelState.IsValid)
        {
            try
            {
                if (!await eventoRepositorio.EditarAsync(evento))
                {
                    return NotFound();
                }

                return RedirigirConExito("Evento actualizado.");
            }
            catch (InvalidOperationException excepcion)
            {
                ModelState.AddModelError(nameof(evento.Aforo), excepcion.Message);
            }
        }
        await CargarCategoriasAsync(evento.CategoriaId);
        return View(evento);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var resumenEvento = await eventoRepositorio.ObtenerAsync(id);
        return resumenEvento == null ? NotFound() : View(resumenEvento);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (!await eventoRepositorio.EliminarAsync(id))
        {
            return NotFound();
        }

        return RedirigirConExito("Evento eliminado de forma lógica.");
    }

    private async Task CargarCategoriasAsync(int? categoriaSeleccionada = null)
    {
        ViewBag.Categorias = new SelectList(await eventoRepositorio.CategoriasAsync(), "Id", "Nombre", categoriaSeleccionada);
    }

    private async Task ValidarCategoriaAsync(Evento evento)
    {
        if (!await eventoRepositorio.CategoriaValidaAsync(evento.CategoriaId))
        {
            ModelState.AddModelError(nameof(evento.CategoriaId), "Selecciona una categoría activa.");
        }
    }

    private IActionResult RedirigirConExito(string mensaje)
    {
        TempData["Exito"] = mensaje;
        return RedirectToAction(nameof(Index));
    }
}
