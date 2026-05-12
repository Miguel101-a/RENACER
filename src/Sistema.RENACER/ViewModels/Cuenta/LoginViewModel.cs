using System.ComponentModel.DataAnnotations;

namespace Sistema.RENACER.ViewModels.Cuenta;

public class LoginViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    [MaxLength(50, ErrorMessage = "Máximo 50 caracteres.")]
    [Display(Name = "Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [MaxLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }
}
