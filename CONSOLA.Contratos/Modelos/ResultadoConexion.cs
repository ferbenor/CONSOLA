namespace CONSOLA.Contratos.Modelos
{
    public class ResultadoConexion
    {
        public bool Exitoso { get; set; }
        public string Servidor { get; set; } = string.Empty;
        public string BaseDatos { get; set; } = string.Empty;
        public string MensajeError { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
        public List<string> Detalles { get; set; } = new();
    }
}
