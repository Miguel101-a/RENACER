namespace Sistema.RENACER.ViewModels.Citas;

public class CitaListaViewModel
{
    public int IdCita { get; set; }
    public string Paciente { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public int DuracionMinutos { get; set; }
    public string TipoCita { get; set; } = string.Empty;
    public string? Profesional { get; set; }
    public string Estado { get; set; } = string.Empty;
}
