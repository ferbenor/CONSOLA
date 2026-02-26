using CONSOLA.Contratos.Modelos;

namespace CONSOLA.Contratos
{
    public interface IBaseDatosServicio
    {
        Task<ResultadoConexion> ProbarConexionAsync();
        Task<List<RegistroTabla>> ConsultarTablaAsync(string nombreTabla, int maxRegistros = 10);
    }
}
