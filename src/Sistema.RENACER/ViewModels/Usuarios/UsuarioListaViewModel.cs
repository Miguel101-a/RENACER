namespace Sistema.RENACER.ViewModels.Usuarios;

public class UsuarioListaViewModel
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public DateTime FechaCreacion { get; set; }
}
