using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sistema.RENACER.ViewModels.Usuarios;

public class UsuarioEditarViewModel
{
    public int IdUsuario { get; set; }

    [Required(ErrorMessage = "Los nombres son requeridos.")]
    [MaxLength(100)]
    [Display(Name = "Nombres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son requeridos.")]
    [MaxLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    [MaxLength(150)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Seleccione un rol.")]
    [Display(Name = "Rol")]
    public int IdRol { get; set; }

    public bool Activo { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public IEnumerable<SelectListItem> Roles { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class CambiarPasswordUsuarioViewModel
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    [MinLength(8, ErrorMessage = "Mínimo 8 caracteres.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Debe tener al menos 1 mayúscula y 1 número.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NuevaPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la contraseña.")]
    [DataType(DataType.Password)]
    [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;
}
