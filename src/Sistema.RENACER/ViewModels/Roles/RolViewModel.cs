using System.ComponentModel.DataAnnotations;

namespace Sistema.RENACER.ViewModels.Roles;

public class RolViewModel
{
    public int IdRol { get; set; }

    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(50, ErrorMessage = "Máximo 50 caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    public List<string> CodigosPermisos { get; set; } = new();
}
