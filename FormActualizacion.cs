using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Velopack;

namespace CONSOLA
{
	/// <summary>
	/// Formulario para mostrar el progreso de descarga e instalación de actualizaciones
	/// </summary>
	public class FormActualizacion : Form
	{
		private ProgressBar progressBar;
		private Label lblEstado;
		private Label lblPorcentaje;
		private Label lblVersion;
		private Button btnCancelar;
		private Panel panelHeader;
		private PictureBox pictureBox;

		private UpdateManager _updateManager;
		private UpdateInfo _updateInfo;
		private bool _descargaCompletada = false;

		public FormActualizacion(UpdateManager updateManager, UpdateInfo updateInfo)
		{
			_updateManager = updateManager;
			_updateInfo = updateInfo;
			InicializarComponentes();
			ConfigurarEventos();
		}

		private void InicializarComponentes()
		{
			// Configuración del formulario
			this.Text = "Actualizando CONSOLA";
			this.Size = new Size(500, 250);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;

			// Panel de encabezado
			panelHeader = new Panel
			{
				Dock = DockStyle.Top,
				Height = 60,
				BackColor = Color.FromArgb(0, 120, 212)
			};

			// Icono (opcional)
			pictureBox = new PictureBox
			{
				Location = new Point(15, 10),
				Size = new Size(40, 40),
				SizeMode = PictureBoxSizeMode.StretchImage
			};
			// Puedes agregar un icono aquí: pictureBox.Image = Properties.Resources.UpdateIcon;

			// Título de versión
			lblVersion = new Label
			{
				Text = $"Actualizando a versión {_updateInfo.TargetFullRelease.Version}",
				Location = new Point(65, 10),
				Size = new Size(400, 40),
				Font = new Font("Segoe UI", 12, FontStyle.Bold),
				ForeColor = Color.White,
				TextAlign = ContentAlignment.MiddleLeft
			};

			panelHeader.Controls.Add(pictureBox);
			panelHeader.Controls.Add(lblVersion);

			// Label de estado
			lblEstado = new Label
			{
				Text = "Preparando descarga...",
				Location = new Point(20, 80),
				Size = new Size(450, 20),
				Font = new Font("Segoe UI", 9),
				ForeColor = Color.FromArgb(64, 64, 64)
			};

			// Barra de progreso
			progressBar = new ProgressBar
			{
				Location = new Point(20, 110),
				Size = new Size(440, 30),
				Style = ProgressBarStyle.Continuous,
				Minimum = 0,
				Maximum = 100
			};

			// Label de porcentaje
			lblPorcentaje = new Label
			{
				Text = "0%",
				Location = new Point(20, 145),
				Size = new Size(440, 20),
				Font = new Font("Segoe UI", 9, FontStyle.Bold),
				ForeColor = Color.FromArgb(0, 120, 212),
				TextAlign = ContentAlignment.MiddleCenter
			};

			// Botón cancelar
			btnCancelar = new Button
			{
				Text = "Cancelar",
				Location = new Point(360, 175),
				Size = new Size(100, 30),
				Font = new Font("Segoe UI", 9)
			};
			btnCancelar.Click += BtnCancelar_Click;

			// Agregar controles al formulario
			this.Controls.Add(panelHeader);
			this.Controls.Add(lblEstado);
			this.Controls.Add(progressBar);
			this.Controls.Add(lblPorcentaje);
			this.Controls.Add(btnCancelar);
		}

		private void ConfigurarEventos()
		{
			// Suscribirse a eventos del UpdateManager
			_updateManager.ProgresoDescarga += (s, porcentaje) =>
			{
				if (InvokeRequired)
				{
					BeginInvoke(new Action(() => ActualizarProgreso(porcentaje)));
				}
				else
				{
					ActualizarProgreso(porcentaje);
				}
			};

			_updateManager.EstadoCambiado += (s, estado) =>
			{
				if (InvokeRequired)
				{
					BeginInvoke(new Action(() => ActualizarEstado(estado)));
				}
				else
				{
					ActualizarEstado(estado);
				}
			};

			// Iniciar descarga automáticamente al cargar
			this.Load += async (s, e) => await IniciarDescargaAsync();
		}

		private void ActualizarProgreso(int porcentaje)
		{
			progressBar.Value = Math.Min(porcentaje, 100);
			lblPorcentaje.Text = $"{porcentaje}%";
		}

		private void ActualizarEstado(string estado)
		{
			lblEstado.Text = estado;
		}

		private async Task IniciarDescargaAsync()
		{
			try
			{
				btnCancelar.Enabled = true;
				ActualizarEstado("Descargando actualización...");

				// Descargar actualización
				var progreso = new Progress<int>(porcentaje => ActualizarProgreso(porcentaje));
				bool exito = await _updateManager.DescargarActualizacionAsync(_updateInfo, progreso);

				if (exito)
				{
					_descargaCompletada = true;
					ActualizarEstado("Descarga completada. Aplicando actualización...");
					btnCancelar.Text = "Cerrar";

					// Esperar 1 segundo para que el usuario vea el mensaje
					await Task.Delay(1000);

					// Aplicar y reiniciar
					_updateManager.AplicarActualizacionYReiniciar(_updateInfo);
				}
				else
				{
					ActualizarEstado("Error al descargar la actualización");
					MessageBox.Show(
						"No se pudo descargar la actualización. Inténtelo más tarde.",
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
					this.DialogResult = DialogResult.Cancel;
					this.Close();
				}
			}
			catch (Exception ex)
			{
				ActualizarEstado($"Error: {ex.Message}");
				MessageBox.Show(
					$"Error al actualizar: {ex.Message}",
					"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			}
		}

		private void BtnCancelar_Click(object? sender, EventArgs e)
		{
			if (_descargaCompletada)
			{
				this.Close();
			}
			else
			{
				var result = MessageBox.Show(
					"¿Está seguro que desea cancelar la actualización?",
					"Confirmar",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question
				);

				if (result == DialogResult.Yes)
				{
					this.DialogResult = DialogResult.Cancel;
					this.Close();
				}
			}
		}
	}
}
