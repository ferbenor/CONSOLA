namespace CONSOLA
{
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
			this.Text = "Diagnostico de Actualizaciones";
			this.Size = new System.Drawing.Size(800, 600);
			this.StartPosition = FormStartPosition.CenterScreen;

			txtLog = new TextBox
			{
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				Dock = DockStyle.Fill,
				ReadOnly = true,
				Font = new System.Drawing.Font("Consolas", 9)
			};

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

				AgregarLog("=== DIAGNOSTICO DE ACTUALIZACIONES ===");
				AgregarLog("");

				var versionActual = _updateManager.ObtenerVersionActual();
				AgregarLog($"1. Version instalada: {versionActual}");
				AgregarLog("");

				var url = _updateManager.ObtenerUrlActualizacion();
				AgregarLog($"2. URL de actualizaciones: {url}");
				AgregarLog("");

				AgregarLog("3. Verificando acceso al servidor...");
				try
				{
					using var client = new System.Net.Http.HttpClient();
					client.Timeout = TimeSpan.FromSeconds(10);
					var response = await client.GetAsync(url + "releases.win.json");
					AgregarLog($"   - Codigo HTTP: {(int)response.StatusCode} {response.StatusCode}");

					if (response.IsSuccessStatusCode)
					{
						var contenido = await response.Content.ReadAsStringAsync();
						AgregarLog($"   - Tamanio respuesta: {contenido.Length} bytes");

						try
						{
							var json = System.Text.Json.JsonDocument.Parse(contenido);
							var assets = json.RootElement.GetProperty("Assets");
							AgregarLog($"   - Versiones en el servidor:");

							var versionesUnicas = new HashSet<string>();
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
				catch (Exception ex)
				{
					AgregarLog($"   - ERROR: {ex.Message}");
				}
				AgregarLog("");

				AgregarLog("4. Verificando actualizaciones con Velopack...");
				try
				{
					var updateInfo = await _updateManager.VerificarActualizacionesAsync();

					if (updateInfo != null)
					{
						AgregarLog($"   - ACTUALIZACION DISPONIBLE!");
						AgregarLog($"   - Version nueva: {updateInfo.TargetFullRelease.Version}");
						AgregarLog($"   - Archivo: {updateInfo.TargetFullRelease.FileName}");
						AgregarLog($"   - Tamanio: {updateInfo.TargetFullRelease.Size / 1024 / 1024} MB");

						if (updateInfo.DeltasToTarget != null && updateInfo.DeltasToTarget.Length > 0)
						{
							var totalDelta = updateInfo.DeltasToTarget.Sum(d => d.Size);
							AgregarLog($"   - Actualizacion delta disponible: {totalDelta / 1024} KB");
						}
					}
					else
					{
						AgregarLog($"   - No hay actualizaciones disponibles");
						AgregarLog($"   - Ya estas en la ultima version");
					}
				}
				catch (Exception ex)
				{
					AgregarLog($"   - ERROR en Velopack: {ex.Message}");
				}

				AgregarLog("");
				AgregarLog("=== FIN DEL DIAGNOSTICO ===");
			}
			finally
			{
				btnVerificar.Enabled = true;
			}
		}
	}
}
