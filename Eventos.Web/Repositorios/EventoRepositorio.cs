using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio(EventosContext contexto) : IEventoRepositorio
{
    private IQueryable<Evento> EventosActivos => contexto.Eventos
            .AsNoTracking()
            .Include(eventoConsultado => eventoConsultado.Categoria)
            .Where(eventoConsultado => eventoConsultado.Activo);

    private static IQueryable<EventoResumen> ProyectarResumen(IQueryable<Evento> consulta)
    {
        return consulta
            .Select(eventoConsultado => new EventoResumen(
                    eventoConsultado,
                    eventoConsultado.Inscripciones.Count(inscripcion => inscripcion.Estado == "Confirmada")));
    }

    public async Task<List<EventoResumen>> ListarAsync(int? categoriaId)
    {
        return await ProyectarResumen(EventosActivos
            .Where(eventoConsultado => categoriaId == null || eventoConsultado.CategoriaId == categoriaId)
            .OrderBy(eventoConsultado => eventoConsultado.Fecha))
            .ToListAsync();
    }

    public async Task<EventoResumen?> ObtenerAsync(int id)
    {
        return await ProyectarResumen(EventosActivos
            .Where(eventoConsultado => eventoConsultado.Id == id))
            .SingleOrDefaultAsync();
    }

    public async Task<List<Categoria>> CategoriasAsync()
    {
        return await contexto.Categorias
            .AsNoTracking()
            .Where(categoria => categoria.Activo)
            .OrderBy(categoria => categoria.Nombre)
            .ToListAsync();
    }

    public async Task<bool> CategoriaValidaAsync(int id)
    {
        return await contexto.Categorias
            .AsNoTracking()
            .AnyAsync(categoria => categoria.Id == id && categoria.Activo);
    }

    public async Task CrearAsync(Evento evento)
    {
        contexto.Eventos.Add(evento);
        await contexto.SaveChangesAsync();
    }

    public async Task<bool> EditarAsync(Evento evento)
    {
        var eventoActual = await contexto.Eventos
            .SingleOrDefaultAsync(eventoConsultado => eventoConsultado.Id == evento.Id && eventoConsultado.Activo);
        if (eventoActual == null)
        {
            return false;
        }

        if (await contexto.Inscripciones
            .AsNoTracking()
            .CountAsync(inscripcion => inscripcion.EventoId == evento.Id && inscripcion.Estado == "Confirmada") > evento.Aforo)
        {
            throw new InvalidOperationException("El aforo no puede ser menor que los inscritos confirmados.");
        }

        eventoActual.Titulo = evento.Titulo;
        eventoActual.Fecha = evento.Fecha;
        eventoActual.Lugar = evento.Lugar;
        eventoActual.Aforo = evento.Aforo;
        eventoActual.Precio = evento.Precio;
        eventoActual.CategoriaId = evento.CategoriaId;
        eventoActual.Modalidad = evento.Modalidad;
        await contexto.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var eventoConsultado = await contexto.Eventos
            .SingleOrDefaultAsync(eventoConsultado => eventoConsultado.Id == id && eventoConsultado.Activo);
        if (eventoConsultado == null)
        {
            return false;
        }

        eventoConsultado.Activo = false;
        await contexto.SaveChangesAsync();
        return true;
    }
}
