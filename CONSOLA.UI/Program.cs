using Velopack;
using CONSOLA.Contratos;
using CONSOLA.MockDatos;

namespace CONSOLA
{
    internal class Program
    {
        public static string rutaActualizaciones = "https://consultas.santarosa.gob.ec/consola/";

        [STAThread]
        static void Main(string[] args)
        {
            VelopackApp.Build().Run();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Para desarrollo (contratado): datos ficticios, sin conexion a Informix
            IBaseDatosServicio servicio = new BaseDatosServicioMock();

            // Para produccion: descomentar la linea de abajo y comentar la de arriba
            // Requiere agregar referencia a CONSOLA.Core en el .csproj
            // IBaseDatosServicio servicio = new CONSOLA.Core.BaseDatosServicio();

            Application.Run(new FormLogin(servicio));
        }
    }
}
