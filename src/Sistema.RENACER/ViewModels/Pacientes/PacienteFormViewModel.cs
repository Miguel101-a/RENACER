using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sistema.RENACER.ViewModels.Pacientes;

public class PacienteFormViewModel
{
    public int IdPaciente { get; set; }

    [Required(ErrorMessage = "Los nombres son requeridos.")]
    [MaxLength(100)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son requeridos.")]
    [MaxLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es requerida.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateTime FechaNacimiento { get; set; } = DateTime.Today.AddYears(-10);

    [MaxLength(20)]
    [Display(Name = "CI / Documento")]
    public string? CI { get; set; }

    [Required(ErrorMessage = "El género es requerido.")]
    [MaxLength(20)]
    [Display(Name = "Género")]
    public string Genero { get; set; } = "Femenino";

    [MaxLength(20)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    [MaxLength(150)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [MaxLength(300)]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    [MaxLength(150)]
    [Display(Name = "Nombre del tutor o responsable")]
    public string? NombreTutor { get; set; }

    [MaxLength(20)]
    [Display(Name = "Teléfono del tutor")]
    public string? TelefonoTutor { get; set; }

    [MaxLength(50)]
    [Display(Name = "Relación con el tutor")]
    public string? RelacionTutor { get; set; }

    [MaxLength(500)]
    [Display(Name = "Motivo de consulta inicial")]
    public string? MotivoConsulta { get; set; }

    public IEnumerable<SelectListItem> Generos { get; set; } = new[]
    {
        new SelectListItem("Femenino",  "Femenino"),
        new SelectListItem("Masculino", "Masculino"),
        new SelectListItem("Otro",      "Otro")
    };
}
