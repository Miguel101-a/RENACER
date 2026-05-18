namespace Sistema.RENACER.ViewModels.Pacientes;

public class PacienteListaViewModel
{
    public int IdPaciente { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string? CI { get; set; }
    public int Edad { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}
