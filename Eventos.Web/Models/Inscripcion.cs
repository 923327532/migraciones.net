using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Eventos.Web.Models;

public class Inscripcion
{
    public int Id { get; set; }

    [Range(1, int.MaxValue), Display(Name = "Evento")]
    public int EventoId { get; set; }

    [ValidateNever]
    public Evento Evento { get; set; } = null!;

    [Range(1, int.MaxValue), Display(Name = "Asistente")]
    public int AsistenteId { get; set; }

    [ValidateNever]
    public Asistente Asistente { get; set; } = null!;

    public DateTime FechaInscripcion { get; set; } = DateTime.Now;

    [Required, StringLength(10), RegularExpression("^(Confirmada|Cancelada)$")]
    public string Estado { get; set; } = "Confirmada";
}
