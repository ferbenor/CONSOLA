using System.Threading.Tasks;
using Velopack;
using CONSOLA.Contratos;

namespace CONSOLA
{
    public partial class FormLogin : Form
    {
        private readonly UpdateManager _updateManager;
        private readonly IBaseDatosServicio _servicio;
        private UpdateInfo? _updateInfoPendiente = null;

        public FormLogin(IBaseDatosServicio servicio)
        {
            _servicio = servicio;
            InitializeComponent();
            _updateManager = new UpdateManager(Program.rutaActualizaciones);
            lblVersion.Text = $"CONSOLA v{_updateManager.ObtenerVersionActual()}";
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await VerificarActualizacionesAsync(mostrarMensajeSiNoHay: false);
        }

        private async void timerActualizaciones_Tick(object sender, EventArgs e)
        {
            await VerificarActualizacionesAsync(mostrarMensajeSiNoHay: false);
        }

        private void btnClickParaActualizar_Click(object sender, EventArgs e)
        {
            if (_updateInfoPendiente != null)
                IniciarDescargaYActualizacion(_updateInfoPendiente);
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            this.Hide();

            // Pasa el servicio al formulario principal
            var formPrincipal = new FormPrincipal(_servicio);
            formPrincipal.FormClosed += (s, args) => this.Close();
            formPrincipal.Show();
        }

        private async Task VerificarActualizacionesAsync(bool mostrarMensajeSiNoHay)
        {
            try
            {
                ActualizarEstado("Verificando actualizaciones...");
                var updateInfo = await _updateManager.VerificarActualizacionesAsync();

                if (updateInfo != null)
                {
                    _updateInfoPendiente = updateInfo;
                    btnClickParaActualizar.Visible = true;
                    var version = updateInfo.TargetFullRelease.Version;
                    ActualizarEstado($"Nueva version {version} disponible");

                    var resultado = MessageBox.Show(
                        $"Nueva version {version} disponible.\n\n¿Desea descargar e instalar ahora?",
                        "Actualizacion Disponible",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (resultado == DialogResult.Yes)
                        IniciarDescargaYActualizacion(updateInfo);
                }
                else
                {
                    _updateInfoPendiente = null;
                    btnClickParaActualizar.Visible = false;
                    ActualizarEstado("La aplicacion esta actualizada");

                    if (mostrarMensajeSiNoHay)
                        MessageBox.Show("Ya tiene la ultima version.", "Sin Actualizaciones",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                ActualizarEstado("Error al verificar actualizaciones");
            }
        }

        private void IniciarDescargaYActualizacion(UpdateInfo updateInfo)
        {
            btnClickParaActualizar.Visible = false;
            using var formActualizacion = new FormActualizacion(_updateManager, updateInfo);
            formActualizacion.ShowDialog(this);
        }

        private void ActualizarEstado(string mensaje)
        {
            if (lblEstado != null)
                lblEstado.Text = mensaje;
        }
    }
}
