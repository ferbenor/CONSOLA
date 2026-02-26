using System.Drawing;
using Velopack;

namespace CONSOLA
{
	public class FormActualizacion : Form
	{
		private ProgressBar progressBar = null!;
		private Label lblEstado = null!;
		private Label lblPorcentaje = null!;
		private Label lblVersion = null!;
		private Button btnCancelar = null!;
		private Panel panelHeader = null!;

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
			this.Text = "Actualizando CONSOLA";
			this.Size = new Size(500, 250);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;

			panelHeader = new Panel
			{
				Dock = DockStyle.Top,
				Height = 60,
				BackColor = Color.FromArgb(0, 120, 212)
			};

			lblVersion = new Label
			{
				Text = $"Actualizando a version {_updateInfo.TargetFullRelease.Version}",
				Location = new Point(20, 10),
				Size = new Size(460, 40),
				Font = new Font("Segoe UI", 12, FontStyle.Bold),
				ForeColor = Color.White,
				TextAlign = ContentAlignment.MiddleLeft
			};

			panelHeader.Controls.Add(lblVersion);

			lblEstado = new Label
			{
				Text = "Preparando descarga...",
				Location = new Point(20, 80),
				Size = new Size(450, 20),
				Font = new Font("Segoe UI", 9),
				ForeColor = Color.FromArgb(64, 64, 64)
			};

			progressBar = new ProgressBar
			{
				Location = new Point(20, 110),
				Size = new Size(440, 30),
				Style = ProgressBarStyle.Continuous,
				Minimum = 0,
				Maximum = 100
			};

			lblPorcentaje = new Label
			{
				Text = "0%",
				Location = new Point(20, 145),
				Size = new Size(440, 20),
				Font = new Font("Segoe UI", 9, FontStyle.Bold),
				ForeColor = Color.FromArgb(0, 120, 212),
				TextAlign = ContentAlignment.MiddleCenter
			};

			btnCancelar = new Button
			{
				Text = "Cancelar",
				Location = new Point(360, 175),
				Size = new Size(100, 30),
				Font = new Font("Segoe UI", 9)
			};
			btnCancelar.Click += BtnCancelar_Click;

			this.Controls.Add(panelHeader);
			this.Controls.Add(lblEstado);
			this.Controls.Add(progressBar);
			this.Controls.Add(lblPorcentaje);
			this.Controls.Add(btnCancelar);
		}

		private void ConfigurarEventos()
		{
			_updateManager.ProgresoDescarga += (s, porcentaje) =>
			{
				if (InvokeRequired)
					BeginInvoke(new Action(() => ActualizarProgreso(porcentaje)));
				else
					ActualizarProgreso(porcentaje);
			};

			_updateManager.EstadoCambiado += (s, estado) =>
			{
				if (InvokeRequired)
					BeginInvoke(new Action(() => ActualizarEstado(estado)));
				else
					ActualizarEstado(estado);
			};

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
				ActualizarEstado("Descargando actualizacion...");

				var progreso = new Progress<int>(porcentaje => ActualizarProgreso(porcentaje));
				bool exito = await _updateManager.DescargarActualizacionAsync(_updateInfo, progreso);

				if (exito)
				{
					_descargaCompletada = true;
					ActualizarEstado("Descarga completada. Aplicando actualizacion...");
					btnCancelar.Text = "Cerrar";
					await Task.Delay(1000);
					_updateManager.AplicarActualizacionYReiniciar(_updateInfo);
				}
				else
				{
					ActualizarEstado("Error al descargar la actualizacion");
					MessageBox.Show("No se pudo descargar la actualizacion. Intentelo mas tarde.",
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.DialogResult = DialogResult.Cancel;
					this.Close();
				}
			}
			catch (Exception ex)
			{
				ActualizarEstado($"Error: {ex.Message}");
				MessageBox.Show($"Error al actualizar: {ex.Message}",
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				var result = MessageBox.Show("¿Esta seguro que desea cancelar la actualizacion?",
					"Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

				if (result == DialogResult.Yes)
				{
					this.DialogResult = DialogResult.Cancel;
					this.Close();
				}
			}
		}
	}
}
