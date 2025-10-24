using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace CONSOLA
{
	/// <summary>
	/// Gestor de actualizaciones usando Velopack
	/// </summary>
	public class UpdateManager
	{
		private readonly string _updateUrl;
		private readonly VelopackApp _app;
		private Velopack.UpdateManager? _velopackManager;

		// Eventos para reportar progreso
		public event EventHandler<int>? ProgresoDescarga;
		public event EventHandler<string>? EstadoCambiado;

		/// <summary>
		/// Constructor del gestor de actualizaciones
		/// </summary>
		/// <param name="updateUrl">URL del servidor donde están los archivos de actualización (ej: http://tu-servidor/instalador/)</param>
		public UpdateManager(string updateUrl)
		{
			_updateUrl = updateUrl;
			_app = VelopackApp.Build();
		}

		private Velopack.UpdateManager ObtenerManager()
		{
			if (_velopackManager == null)
			{
				_velopackManager = new Velopack.UpdateManager(new SimpleWebSource(_updateUrl));
			}
			return _velopackManager;
		}

		/// <summary>
		/// Verifica si hay actualizaciones disponibles
		/// </summary>
		/// <returns>Información de actualización o null si no hay actualizaciones</returns>
		public async Task<UpdateInfo?> VerificarActualizacionesAsync()
		{
			try
			{
				EstadoCambiado?.Invoke(this, "Verificando actualizaciones...");
				var updateManager = ObtenerManager();
				var updateInfo = await updateManager.CheckForUpdatesAsync();

				if (updateInfo != null)
				{
					EstadoCambiado?.Invoke(this, $"Actualización disponible: v{updateInfo.TargetFullRelease.Version}");
				}
				else
				{
					EstadoCambiado?.Invoke(this, "No hay actualizaciones disponibles");
				}

				return updateInfo;
			}
			catch (Exception ex)
			{
				EstadoCambiado?.Invoke(this, $"Error: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// Descarga la actualización con reporte de progreso
		/// </summary>
		public async Task<bool> DescargarActualizacionAsync(UpdateInfo updateInfo, IProgress<int>? progreso = null)
		{
			try
			{
				EstadoCambiado?.Invoke(this, "Descargando actualización...");
				var updateManager = ObtenerManager();

				// Descargar con callback de progreso
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

		/// <summary>
		/// Aplica la actualización y reinicia la aplicación
		/// </summary>
		public void AplicarActualizacionYReiniciar(UpdateInfo updateInfo)
		{
			try
			{
				EstadoCambiado?.Invoke(this, "Aplicando actualización y reiniciando...");
				var updateManager = ObtenerManager();
				updateManager.ApplyUpdatesAndRestart(updateInfo);
			}
			catch (Exception ex)
			{
				EstadoCambiado?.Invoke(this, $"Error al aplicar actualización: {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// Obtiene la versión actual de la aplicación
		/// </summary>
		public string ObtenerVersionActual()
		{
			try
			{
				var updateManager = ObtenerManager();
				return updateManager.CurrentVersion?.ToString() ?? "1.0.0";
			}
			catch
			{
				return "1.0.0";
			}
		}

		/// <summary>
		/// Obtiene la URL de actualización configurada
		/// </summary>
		public string ObtenerUrlActualizacion()
		{
			return _updateUrl;
		}
	}
}
