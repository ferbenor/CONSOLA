
using System;
using System.Windows.Forms;
using Velopack;

namespace CONSOLA
{
	internal class Program
	{
		public static string rutaActualizaciones = "https://consultas.santarosa.gob.ec/consola/";
		[STAThread]
		static void Main(string[] args)
		{
			// IMPORTANTE: Configurar Velopack ANTES de cualquier otra cosa
			// Esto maneja instalación, desinstalación y actualizaciones
			VelopackApp.Build().Run();

			// Configurar aplicación WinForms
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Iniciar con el formulario de Login
			Application.Run(new FormLogin());
		}
	}
}

