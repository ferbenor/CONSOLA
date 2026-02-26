using Velopack;
using CONSOLA.Contratos;
#if DEBUG
using CONSOLA.MockDatos;
#endif

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

#if DEBUG
            // Debug: datos ficticios - el contratado siempre trabaja asi
            IBaseDatosServicio servicio = new BaseDatosServicioMock();
#else
            // Release: conexion real a Informix - solo para produccion
            IBaseDatosServicio servicio = new CONSOLA.Core.BaseDatosServicio();
#endif

            Application.Run(new FormLogin(servicio));
        }
    }
}
