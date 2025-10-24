using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Velopack;

namespace CONSOLA
{
	/// <summary>
	/// Formulario de login con verificación automática de actualizaciones
	/// </summary>
	public partial class FormLogin : Form
	{
		private readonly UpdateManager _updateManager;
		private UpdateInfo? _updateInfoPendiente = null;

		public FormLogin()
		{
			InitializeComponent();

			// Configurar el gestor de actualizaciones
			_updateManager = new UpdateManager(Program.rutaActualizaciones);

			// Mostrar versión actual en el header
			lblVersion.Text = $"CONSOLA v{_updateManager.ObtenerVersionActual()}";
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
		/// Botón: Click para Actualizar (solo visible cuando hay actualización pendiente)
		/// </summary>
		private void btnClickParaActualizar_Click(object sender, EventArgs e)
		{
			if (_updateInfoPendiente != null)
			{
				IniciarDescargaYActualizacion(_updateInfoPendiente);
			}
		}

		/// <summary>
		/// Botón: Entrar - abre el formulario principal
		/// </summary>
		private void btnEntrar_Click(object sender, EventArgs e)
		{
			// Validar campos (opcional - puedes agregar tu lógica de validación)
			//if (string.IsNullOrWhiteSpace(txtUsuario.Text))
			//{
			//	MessageBox.Show(
			//		"Por favor ingrese un usuario.",
			//		"Campo Requerido",
			//		MessageBoxButtons.OK,
			//		MessageBoxIcon.Warning
			//	);
			//	txtUsuario.Focus();
			//	return;
			//}

			//if (string.IsNullOrWhiteSpace(txtPassword.Text))
			//{
			//	MessageBox.Show(
			//		"Por favor ingrese una contraseña.",
			//		"Campo Requerido",
			//		MessageBoxButtons.OK,
			//		MessageBoxIcon.Warning
			//	);
			//	txtPassword.Focus();
			//	return;
			//}

			// Aquí puedes agregar tu lógica de autenticación
			// Por ahora simplemente abrimos el formulario principal

			// Ocultar el formulario de login
			this.Hide();

			// Abrir el formulario principal
			var formPrincipal = new FormPrincipal();
			formPrincipal.FormClosed += (s, args) =>
			{
				// Cuando se cierra el formulario principal, cerrar toda la aplicación
				this.Close();
			};
			formPrincipal.Show();
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
					btnClickParaActualizar.Visible = true;

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
					btnClickParaActualizar.Visible = false;
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
				ActualizarEstado($"Error al verificar actualizaciones");

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
			// Ocultar el botón de actualización mientras se descarga
			btnClickParaActualizar.Visible = false;

			// Mostrar formulario con barra de progreso
			using (var formActualizacion = new FormActualizacion(_updateManager, updateInfo))
			{
				formActualizacion.ShowDialog(this);
			}
		}

		/// <summary>
		/// Actualiza el mensaje de estado
		/// </summary>
		private void ActualizarEstado(string mensaje)
		{
			if (lblEstado != null)
			{
				lblEstado.Text = mensaje;
			}
		}
	}
}
