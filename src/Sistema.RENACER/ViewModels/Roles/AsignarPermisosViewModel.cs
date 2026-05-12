namespace Sistema.RENACER.ViewModels.Roles;

public class PermisoCheckItem
{
    public int IdPermiso { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public bool Asignado { get; set; }
}

public class AsignarPermisosViewModel
{
    public int IdRol { get; set; }
    public string NombreRol { get; set; } = string.Empty;
    public List<PermisoCheckItem> Permisos { get; set; } = new();
    public List<int> PermisosSeleccionados { get; set; } = new();
}
