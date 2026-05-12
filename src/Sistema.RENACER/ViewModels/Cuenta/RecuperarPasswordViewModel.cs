using System.ComponentModel.DataAnnotations;

namespace Sistema.RENACER.ViewModels.Cuenta;

public class RecuperarPasswordViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    [MaxLength(50)]
    [Display(Name = "Nombre de usuario")]
    public string NombreUsuario { get; set; } = string.Empty;
}
