namespace Sistema.RENACER.ViewModels.Pacientes;

public class PacienteDetalleViewModel
{
    public int IdPaciente { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public int Edad { get; set; }
    public string? Genero { get; set; }
    public string? CI { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string? NombreTutor { get; set; }
    public string? TelefonoTutor { get; set; }
    public string? RelacionTutor { get; set; }
    public string? MotivoConsulta { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}
