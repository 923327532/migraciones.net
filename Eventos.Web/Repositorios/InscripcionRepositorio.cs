using System.Data;
using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio(EventosContext contexto) : IInscripcionRepositorio
{
    public async Task<List<Inscripcion>> ListarAsync()
    {
        return await contexto.Inscripciones
            .AsNoTracking()
            .Include(inscripcionConsultada => inscripcionConsultada.Evento)
            .Include(inscripcionConsultada => inscripcionConsultada.Asistente)
            .OrderByDescending(inscripcionConsultada => inscripcionConsultada.FechaInscripcion)
            .ToListAsync();
    }

    public async Task<string?> CrearAsync(Inscripcion inscripcion)
    {
        // La transacción evita exceder el aforo o duplicar inscripciones simultáneas.
        await using var transaccion = await contexto.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var evento = await contexto.Eventos
            .AsNoTracking()
            .SingleOrDefaultAsync(eventoConsultado => eventoConsultado.Id == inscripcion.EventoId && eventoConsultado.Activo);
        if (evento == null)
        {
            return "Selecciona un evento activo.";
        }

        if (evento.Fecha <= DateTime.Now)
        {
            return "El evento ya ha pasado.";
        }

        if (!await contexto.Asistentes
            .AsNoTracking()
            .AnyAsync(asistente => asistente.Id == inscripcion.AsistenteId && asistente.Activo))
        {
            return "Selecciona un asistente activo.";
        }

        var inscripcionesConfirmadas = contexto.Inscripciones
            .AsNoTracking()
            .Where(inscripcionConsultada => inscripcionConsultada.EventoId == inscripcion.EventoId && inscripcionConsultada.Estado == "Confirmada");
        if (await inscripcionesConfirmadas
            .AnyAsync(inscripcionConsultada => inscripcionConsultada.AsistenteId == inscripcion.AsistenteId))
        {
            return "El asistente ya tiene una inscripción confirmada en este evento.";
        }

        if (await inscripcionesConfirmadas
            .CountAsync() >= evento.Aforo)
        {
            return "El aforo está completo.";
        }

        inscripcion.Estado = "Confirmada";
        inscripcion.FechaInscripcion = DateTime.Now;
        contexto.Inscripciones.Add(inscripcion);
        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();
        return null;
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcionConsultada = await contexto.Inscripciones
            .SingleOrDefaultAsync(inscripcionConsultada => inscripcionConsultada.Id == id && inscripcionConsultada.Estado == "Confirmada");
        if (inscripcionConsultada == null)
        {
            return false;
        }

        inscripcionConsultada.Estado = "Cancelada";
        await contexto.SaveChangesAsync();
        return true;
    }
}
