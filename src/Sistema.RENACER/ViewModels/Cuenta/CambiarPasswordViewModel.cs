using System.ComponentModel.DataAnnotations;

namespace Sistema.RENACER.ViewModels.Cuenta;

public class CambiarPasswordViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    [MinLength(8, ErrorMessage = "Mínimo 8 caracteres.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Debe contener al menos 1 mayúscula y 1 número.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NuevaPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la contraseña.")]
    [DataType(DataType.Password)]
    [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;
}
