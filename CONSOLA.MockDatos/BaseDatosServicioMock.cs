using System.Text.Json;
using CONSOLA.Contratos;
using CONSOLA.Contratos.Modelos;

namespace CONSOLA.MockDatos
{
    public class BaseDatosServicioMock : IBaseDatosServicio
    {
        private readonly string _rutaDatos;

        public BaseDatosServicioMock()
        {
            _rutaDatos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datos");
        }

        public async Task<ResultadoConexion> ProbarConexionAsync()
        {
            await Task.Delay(500); // simular latencia de red

            return new ResultadoConexion
            {
                Exitoso = true,
                Servidor = "mock-server (datos ficticios)",
                BaseDatos = "mock-db",
                TotalRegistros = 2,
                Detalles = new List<string>
                {
                    "Conexion simulada exitosamente",
                    "Usando datos ficticios del archivo exempres.json",
                    "Registro 1: EMP-001 Empresa Demo Uno",
                    "Registro 2: EMP-002 Empresa Demo Dos",
                    "Total registros: 2"
                }
            };
        }

        public async Task<List<RegistroTabla>> ConsultarTablaAsync(string nombreTabla, int maxRegistros = 10)
        {
            await Task.Delay(300); // simular latencia

            var archivo = Path.Combine(_rutaDatos, $"{nombreTabla}.json");
            if (!File.Exists(archivo))
                return new List<RegistroTabla>();

            var json = await File.ReadAllTextAsync(archivo);
            return JsonSerializer.Deserialize<List<RegistroTabla>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<RegistroTabla>();
        }
    }
}
