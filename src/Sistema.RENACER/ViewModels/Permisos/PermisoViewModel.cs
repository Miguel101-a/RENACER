using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sistema.RENACER.ViewModels.Permisos;

public class PermisoListaViewModel
{
    public int IdPermiso { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int CantidadRoles { get; set; }
}

public class PermisoCrearViewModel
{
    [Required(ErrorMessage = "El código es requerido.")]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z0-9_]+$",
        ErrorMessage = "Solo letras mayúsculas, números y guión bajo.")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(300)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La categoría es requerida.")]
    [Display(Name = "Categoría")]
    public string Categoria { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> Categorias { get; set; } = new List<SelectListItem>
    {
        new("Técnico", "Técnico"),
        new("Operativo", "Operativo")
    };
}

public class PermisoEditarViewModel
{
    public int IdPermiso { get; set; }
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(300)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
}
