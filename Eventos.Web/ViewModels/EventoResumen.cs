using Eventos.Web.Models;

namespace Eventos.Web.ViewModels;

public record EventoResumen(Evento Evento, int Confirmados)
{
    public int Disponibles => Evento.Aforo - Confirmados;
}
