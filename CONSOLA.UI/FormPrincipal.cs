using System.Text;
using System.Threading.Tasks;
using Velopack;
using CONSOLA.Contratos;

namespace CONSOLA
{
    public partial class FormPrincipal : Form
    {
        private readonly UpdateManager _updateManager;
        private readonly IBaseDatosServicio _servicio;
        private UpdateInfo? _updateInfoPendiente = null;

        public FormPrincipal(IBaseDatosServicio servicio)
        {
            _servicio = servicio;
            InitializeComponent();
            _updateManager = new UpdateManager(Program.rutaActualizaciones);
            this.Text = $"CONSOLA - Version {_updateManager.ObtenerVersionActual()}";
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

        private async void menuBuscarActualizaciones_Click(object sender, EventArgs e)
        {
            await VerificarActualizacionesAsync(mostrarMensajeSiNoHay: true);
        }

        private void menuDiagnosticoActualizaciones_Click(object sender, EventArgs e)
        {
            var formDiagnostico = new DiagnosticoActualizacion();
            formDiagnostico.ShowDialog(this);
        }

        private void menuClickParaActualizar_Click(object sender, EventArgs e)
        {
            if (_updateInfoPendiente != null)
                IniciarDescargaYActualizacion(_updateInfoPendiente);
        }

        // Sin una sola linea de DB2 - el contratado solo ve esta logica de UI
        private async void btnProbarConexion_Click(object sender, EventArgs e)
        {
            txtResultado.Clear();
            btnProbarConexion.Enabled = false;
            ActualizarEstado("Probando conexion...");

            var texto = new StringBuilder();
            texto.AppendLine("=== PRUEBA DE CONEXION ===");
            texto.AppendLine($"Fecha/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            texto.AppendLine();

            try
            {
                var resultado = await _servicio.ProbarConexionAsync();

                if (resultado.Exitoso)
                {
                    texto.AppendLine($"Servidor   : {resultado.Servidor}");
                    texto.AppendLine($"Base datos : {resultado.BaseDatos}");
                    texto.AppendLine();

                    foreach (var detalle in resultado.Detalles)
                        texto.AppendLine($"  {detalle}");

                    texto.AppendLine();
                    texto.AppendLine("=== PRUEBA COMPLETADA EXITOSAMENTE ===");
                    ActualizarEstado($"Conexion exitosa - {resultado.TotalRegistros} registros");
                }
                else
                {
                    texto.AppendLine($"ERROR: {resultado.MensajeError}");
                    texto.AppendLine();
                    texto.AppendLine("=== PRUEBA FINALIZADA CON ERRORES ===");
                    ActualizarEstado("Error en la conexion");
                }
            }
            catch (Exception ex)
            {
                texto.AppendLine($"Error inesperado: {ex.Message}");
                ActualizarEstado("Error inesperado");
            }

            txtResultado.Text = texto.ToString();
            btnProbarConexion.Enabled = true;
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
                    menuClickParaActualizar.Visible = true;
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
                    menuClickParaActualizar.Visible = false;
                    ActualizarEstado("La aplicacion esta actualizada");

                    if (mostrarMensajeSiNoHay)
                        MessageBox.Show("Ya tiene la ultima version.", "Sin Actualizaciones",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ActualizarEstado($"Error al verificar actualizaciones: {ex.Message}");
                if (mostrarMensajeSiNoHay)
                    MessageBox.Show($"No se pudo verificar actualizaciones:\n{ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IniciarDescargaYActualizacion(UpdateInfo updateInfo)
        {
            menuClickParaActualizar.Visible = false;
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
