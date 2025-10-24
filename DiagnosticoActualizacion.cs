using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace CONSOLA
{
	/// <summary>
	/// Formulario de diagnóstico para verificar la detección de actualizaciones
	/// </summary>
	public class DiagnosticoActualizacion : Form
	{
		private TextBox txtLog = null!;
		private Button btnVerificar = null!;
		private UpdateManager _updateManager = null!;

		public DiagnosticoActualizacion()
		{
			InicializarComponentes();
			_updateManager = new UpdateManager(Program.rutaActualizaciones);
		}

		private void InicializarComponentes()
		{
			this.Text = "Diagnóstico de Actualizaciones";
			this.Size = new System.Drawing.Size(800, 600);
			this.StartPosition = FormStartPosition.CenterScreen;

			// TextBox para log
			txtLog = new TextBox
			{
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				Dock = DockStyle.Fill,
				ReadOnly = true,
				Font = new System.Drawing.Font("Consolas", 9)
			};

			// Botón para verificar
			btnVerificar = new Button
			{
				Text = "Verificar Actualizaciones",
				Dock = DockStyle.Top,
				Height = 40
			};
			btnVerificar.Click += async (s, e) => await VerificarActualizaciones();

			this.Controls.Add(txtLog);
			this.Controls.Add(btnVerificar);
		}

		private void AgregarLog(string mensaje)
		{
			if (txtLog.InvokeRequired)
			{
				txtLog.Invoke(new Action(() => AgregarLog(mensaje)));
				return;
			}

			txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {mensaje}\r\n");
		}

		private async Task VerificarActualizaciones()
		{
			try
			{
				btnVerificar.Enabled = false;
				txtLog.Clear();

				AgregarLog("=== DIAGNÓSTICO DE ACTUALIZACIONES ===");
				AgregarLog("");

				// 1. Versión actual
				var versionActual = _updateManager.ObtenerVersionActual();
				AgregarLog($"1. Versión instalada: {versionActual}");
				AgregarLog("");

				// 2. URL de actualización
				var url = _updateManager.ObtenerUrlActualizacion();
				AgregarLog($"2. URL de actualizaciones: {url}");
				AgregarLog("");

				// 3. Verificar acceso al servidor
				AgregarLog("3. Verificando acceso al servidor...");
				try
				{
					using (var client = new System.Net.Http.HttpClient())
					{
						client.Timeout = TimeSpan.FromSeconds(10);
						var response = await client.GetAsync(url + "releases.win.json");
						AgregarLog($"   - Código HTTP: {(int)response.StatusCode} {response.StatusCode}");

						if (response.IsSuccessStatusCode)
						{
							var contenido = await response.Content.ReadAsStringAsync();
							AgregarLog($"   - Tamaño respuesta: {contenido.Length} bytes");

							// Parsear JSON para ver versiones disponibles
							try
							{
								var json = System.Text.Json.JsonDocument.Parse(contenido);
								var assets = json.RootElement.GetProperty("Assets");
								AgregarLog($"   - Versiones en el servidor:");

								var versionesUnicas = new System.Collections.Generic.HashSet<string>();
								foreach (var asset in assets.EnumerateArray())
								{
									var version = asset.GetProperty("Version").GetString();
									if (version != null && versionesUnicas.Add(version))
									{
										var tipo = asset.GetProperty("Type").GetString();
										AgregarLog($"     * v{version} ({tipo})");
									}
								}
							}
							catch (Exception ex)
							{
								AgregarLog($"   - Error al parsear JSON: {ex.Message}");
							}
						}
						else
						{
							AgregarLog($"   - ERROR: No se pudo acceder al servidor");
						}
					}
				}
				catch (Exception ex)
				{
					AgregarLog($"   - ERROR: {ex.Message}");
				}
				AgregarLog("");

				// 4. Verificar actualizaciones con Velopack
				AgregarLog("4. Verificando actualizaciones con Velopack...");
				try
				{
					var updateInfo = await _updateManager.VerificarActualizacionesAsync();

					if (updateInfo != null)
					{
						AgregarLog($"   - ¡ACTUALIZACIÓN DISPONIBLE!");
						AgregarLog($"   - Versión nueva: {updateInfo.TargetFullRelease.Version}");
						AgregarLog($"   - Archivo: {updateInfo.TargetFullRelease.FileName}");
						AgregarLog($"   - Tamaño: {updateInfo.TargetFullRelease.Size / 1024 / 1024} MB");

						if (updateInfo.DeltasToTarget != null && updateInfo.DeltasToTarget.Length > 0)
						{
							var totalDelta = 0L;
							foreach (var delta in updateInfo.DeltasToTarget)
							{
								totalDelta += delta.Size;
							}
							AgregarLog($"   - Actualización delta disponible: {totalDelta / 1024} KB");
						}
					}
					else
					{
						AgregarLog($"   - No hay actualizaciones disponibles");
						AgregarLog($"   - Ya estás en la última versión");
					}
				}
				catch (Exception ex)
				{
					AgregarLog($"   - ERROR en Velopack: {ex.Message}");
					AgregarLog($"   - StackTrace: {ex.StackTrace}");
				}

				AgregarLog("");
				AgregarLog("=== FIN DEL DIAGNÓSTICO ===");
			}
			finally
			{
				btnVerificar.Enabled = true;
			}
		}
	}
}
