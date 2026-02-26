using Velopack;
using Velopack.Sources;

namespace CONSOLA
{
	public class UpdateManager
	{
		private readonly string _updateUrl;
		private Velopack.UpdateManager? _velopackManager;

		public event EventHandler<int>? ProgresoDescarga;
		public event EventHandler<string>? EstadoCambiado;

		public UpdateManager(string updateUrl)
		{
			_updateUrl = updateUrl;
		}

		private Velopack.UpdateManager ObtenerManager()
		{
			if (_velopackManager == null)
				_velopackManager = new Velopack.UpdateManager(new SimpleWebSource(_updateUrl));
			return _velopackManager;
		}

		public async Task<UpdateInfo?> VerificarActualizacionesAsync()
		{
			try
			{
				EstadoCambiado?.Invoke(this, "Verificando actualizaciones...");
				var updateManager = ObtenerManager();
				var updateInfo = await updateManager.CheckForUpdatesAsync();

				if (updateInfo != null)
					EstadoCambiado?.Invoke(this, $"Actualizacion disponible: v{updateInfo.TargetFullRelease.Version}");
				else
					EstadoCambiado?.Invoke(this, "No hay actualizaciones disponibles");

				return updateInfo;
			}
			catch (Exception ex)
			{
				EstadoCambiado?.Invoke(this, $"Error: {ex.Message}");
				return null;
			}
		}

		public async Task<bool> DescargarActualizacionAsync(UpdateInfo updateInfo, IProgress<int>? progreso = null)
		{
			try
			{
				EstadoCambiado?.Invoke(this, "Descargando actualizacion...");
				var updateManager = ObtenerManager();

				await updateManager.DownloadUpdatesAsync(updateInfo, progress => {
					var porcentaje = (int)progress;
					ProgresoDescarga?.Invoke(this, porcentaje);
					progreso?.Report(porcentaje);
				});

				EstadoCambiado?.Invoke(this, "Descarga completada");
				return true;
			}
			catch (Exception ex)
			{
				EstadoCambiado?.Invoke(this, $"Error al descargar: {ex.Message}");
				return false;
			}
		}

		public void AplicarActualizacionYReiniciar(UpdateInfo updateInfo)
		{
			EstadoCambiado?.Invoke(this, "Aplicando actualizacion y reiniciando...");
			var updateManager = ObtenerManager();
			updateManager.ApplyUpdatesAndRestart(updateInfo);
		}

		public string ObtenerVersionActual()
		{
			try
			{
				return ObtenerManager().CurrentVersion?.ToString() ?? "1.0.0";
			}
			catch
			{
				return "1.0.0";
			}
		}

		public string ObtenerUrlActualizacion() => _updateUrl;
	}
}
