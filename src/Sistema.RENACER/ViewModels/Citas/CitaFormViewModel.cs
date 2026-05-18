using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sistema.RENACER.ViewModels.Citas;

public class CitaFormViewModel
{
    public int IdCita { get; set; }

    [Required(ErrorMessage = "Seleccione un paciente.")]
    [Display(Name = "Paciente")]
    public int IdPaciente { get; set; }

    [Required(ErrorMessage = "La fecha de la cita es requerida.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "La hora de inicio es requerida.")]
    [DataType(DataType.Time)]
    [Display(Name = "Hora inicio")]
    public TimeSpan HoraInicio { get; set; } = new TimeSpan(9, 0, 0);

    [Required(ErrorMessage = "La hora de fin es requerida.")]
    [DataType(DataType.Time)]
    [Display(Name = "Hora fin")]
    public TimeSpan HoraFin { get; set; } = new TimeSpan(10, 0, 0);

    [Required(ErrorMessage = "Seleccione un tipo de cita.")]
    [Display(Name = "Tipo de cita")]
    public int IdTipoCita { get; set; }

    [Required(ErrorMessage = "Seleccione un servicio.")]
    [Display(Name = "Servicio")]
    public int IdServicio { get; set; }

    [Display(Name = "Profesional asignada")]
    public int? IdProfesional { get; set; }

    [MaxLength(500)]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public IEnumerable<SelectListItem> Pacientes      { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> TiposCita      { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Servicios      { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Profesionales  { get; set; } = Enumerable.Empty<SelectListItem>();

    public int DuracionMinutos =>
        (int)(HoraFin - HoraInicio).TotalMinutes;

    public DateTime FechaHora =>
        Fecha.Date.Add(HoraInicio);
}
