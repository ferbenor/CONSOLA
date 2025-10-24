using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IBM.Data.Db2;
using Velopack;

namespace CONSOLA
{
	/// <summary>
	/// Formulario principal con gestión automática de actualizaciones
	/// </summary>
	public partial class FormPrincipal : Form
	{
		private readonly UpdateManager _updateManager;
		private UpdateInfo? _updateInfoPendiente = null;

		public FormPrincipal()
		{
			InitializeComponent();

			// Configurar el gestor de actualizaciones con la URL de tu servidor IIS
			_updateManager = new UpdateManager(Program.rutaActualizaciones);

			// Mostrar versión actual en el título
			this.Text = $"CONSOLA - Versión {_updateManager.ObtenerVersionActual()}";
		}

		/// <summary>
		/// Evento al cargar el formulario - verificar actualizaciones automáticamente
		/// </summary>
		protected override async void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// Verificar actualizaciones al iniciar
			await VerificarActualizacionesAsync(mostrarMensajeSiNoHay: false);
		}

		/// <summary>
		/// Timer - verifica actualizaciones cada 5 minutos
		/// </summary>
		private async void timerActualizaciones_Tick(object sender, EventArgs e)
		{
			await VerificarActualizacionesAsync(mostrarMensajeSiNoHay: false);
		}

		/// <summary>
		/// Menu: Ayuda → Buscar Actualizaciones
		/// </summary>
		private async void menuBuscarActualizaciones_Click(object sender, EventArgs e)
		{
			await VerificarActualizacionesAsync(mostrarMensajeSiNoHay: true);
		}

		/// <summary>
		/// Menu: Ayuda → Diagnóstico de Actualizaciones
		/// </summary>
		private void menuDiagnosticoActualizaciones_Click(object sender, EventArgs e)
		{
			var formDiagnostico = new DiagnosticoActualizacion();
			formDiagnostico.ShowDialog(this);
		}

		/// <summary>
		/// Menu: Click para Actualizar (solo visible cuando hay actualización pendiente)
		/// </summary>
		private void menuClickParaActualizar_Click(object sender, EventArgs e)
		{
			if (_updateInfoPendiente != null)
			{
				IniciarDescargaYActualizacion(_updateInfoPendiente);
			}
		}

		/// <summary>
		/// Verifica si hay actualizaciones disponibles
		/// </summary>
		private async Task VerificarActualizacionesAsync(bool mostrarMensajeSiNoHay)
		{
			try
			{
				ActualizarEstado("Verificando actualizaciones...");

				var updateInfo = await _updateManager.VerificarActualizacionesAsync();

				if (updateInfo != null)
				{
					// Hay actualización disponible
					_updateInfoPendiente = updateInfo;
					menuClickParaActualizar.Visible = true;

					var version = updateInfo.TargetFullRelease.Version;
					ActualizarEstado($"Nueva versión {version} disponible");

					var resultado = MessageBox.Show(
						$"Nueva versión {version} disponible.\n\n¿Desea descargar e instalar ahora?",
						"Actualización Disponible",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Information
					);

					if (resultado == DialogResult.Yes)
					{
						IniciarDescargaYActualizacion(updateInfo);
					}
				}
				else
				{
					// No hay actualizaciones
					_updateInfoPendiente = null;
					menuClickParaActualizar.Visible = false;
					ActualizarEstado("La aplicación está actualizada");

					if (mostrarMensajeSiNoHay)
					{
						MessageBox.Show(
							"Ya tiene la última versión de la aplicación.",
							"Sin Actualizaciones",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information
						);
					}
				}
			}
			catch (Exception ex)
			{
				ActualizarEstado($"Error al verificar actualizaciones: {ex.Message}");

				if (mostrarMensajeSiNoHay)
				{
					MessageBox.Show(
						$"No se pudo verificar actualizaciones:\n{ex.Message}",
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}
		}

		/// <summary>
		/// Inicia el proceso de descarga con barra de progreso
		/// </summary>
		private void IniciarDescargaYActualizacion(UpdateInfo updateInfo)
		{
			// Ocultar el menú de actualización mientras se descarga
			menuClickParaActualizar.Visible = false;

			// Mostrar formulario con barra de progreso
			using (var formActualizacion = new FormActualizacion(_updateManager, updateInfo))
			{
				formActualizacion.ShowDialog(this);
			}
		}

		/// <summary>
		/// Actualiza el mensaje de estado en la barra inferior
		/// </summary>
		private void ActualizarEstado(string mensaje)
		{
			if (lblEstado != null)
			{
				lblEstado.Text = mensaje;
			}
		}

		/// <summary>
		/// Botón: Probar Conexión Informix
		/// </summary>
		private async void btnProbarConexion_Click(object sender, EventArgs e)
		{
			txtResultado.Clear();
			btnProbarConexion.Enabled = false;
			ActualizarEstado("Probando conexión a Informix...");

			var resultado = new StringBuilder();
			resultado.AppendLine("=== PRUEBA DE CONEXIÓN INFORMIX ===");
			resultado.AppendLine($"Fecha/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
			resultado.AppendLine();

			await Task.Run(() =>
			{
				try
				{
					// Cadena de conexión - AJUSTA ESTOS VALORES A TU SERVIDOR
					string cadenaConexion = "Database=exgadmsr;Server=192.168.100.18:28157;UserID=informix;Password=informix;";

					resultado.AppendLine($"Cadena de conexión: {cadenaConexion.Replace("Password=informix", "Password=***")}");
					resultado.AppendLine();
					resultado.AppendLine("Conectando a la base de datos...");

					using (DB2Connection conexion = new DB2Connection(cadenaConexion))
					{
						conexion.Open();
						resultado.AppendLine("✓ Conexión establecida exitosamente");
						resultado.AppendLine($"Estado: {conexion.State}");
						resultado.AppendLine($"Servidor: {conexion.DataSource}");
						resultado.AppendLine($"Base de datos: {conexion.Database}");
						resultado.AppendLine();

						// Ejecutar consulta de prueba
						resultado.AppendLine("Ejecutando consulta: SELECT * FROM exempres");

						using (DB2Command comando = new DB2Command("SELECT * FROM exempres", conexion))
						{
							var reader = comando.ExecuteReader();

							int contador = 0;
							resultado.AppendLine();
							resultado.AppendLine("Registros encontrados:");
							resultado.AppendLine("----------------------------------------");

							// Mostrar los primeros 10 registros
							while (reader.Read() && contador < 10)
							{
								// Asumiendo que el primer campo es un ID
								var id = reader.GetValue(0);
								resultado.AppendLine($"  Registro {contador + 1}: ID = {id}");
								contador++;
							}

							// Contar el resto de registros
							while (reader.Read())
							{
								contador++;
							}

							resultado.AppendLine("----------------------------------------");
							resultado.AppendLine();
							resultado.AppendLine($"✓ Total de registros leídos: {contador}");
						}

						conexion.Close();
						resultado.AppendLine();
						resultado.AppendLine("✓ Conexión cerrada correctamente");
					}

					resultado.AppendLine();
					resultado.AppendLine("=== PRUEBA COMPLETADA EXITOSAMENTE ===");
				}
				catch (Exception ex)
				{
					resultado.AppendLine();
					resultado.AppendLine("✗ ERROR AL CONECTAR:");
					resultado.AppendLine($"Tipo: {ex.GetType().Name}");
					resultado.AppendLine($"Mensaje: {ex.Message}");

					if (ex.InnerException != null)
					{
						resultado.AppendLine();
						resultado.AppendLine("Error interno:");
						resultado.AppendLine($"  {ex.InnerException.Message}");
					}

					resultado.AppendLine();
					resultado.AppendLine("=== PRUEBA FINALIZADA CON ERRORES ===");
				}
			});

			// Mostrar resultado en el TextBox
			txtResultado.Text = resultado.ToString();
			btnProbarConexion.Enabled = true;
			ActualizarEstado("Prueba de conexión finalizada");
		}
	}
}
