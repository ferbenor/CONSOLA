namespace CONSOLA
{
	partial class FormPrincipal
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Código generado por el Diseñador de Windows Forms

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.menuAyuda = new System.Windows.Forms.ToolStripMenuItem();
			this.menuBuscarActualizaciones = new System.Windows.Forms.ToolStripMenuItem();
			this.menuDiagnosticoActualizaciones = new System.Windows.Forms.ToolStripMenuItem();
			this.menuClickParaActualizar = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.lblEstado = new System.Windows.Forms.ToolStripStatusLabel();
			this.timerActualizaciones = new System.Windows.Forms.Timer(this.components);
			this.btnProbarConexion = new System.Windows.Forms.Button();
			this.txtResultado = new System.Windows.Forms.TextBox();
			this.lblTitulo = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			//
			// menuStrip1
			//
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAyuda,
            this.menuClickParaActualizar});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(800, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			//
			// menuAyuda
			//
			this.menuAyuda.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBuscarActualizaciones,
            this.menuDiagnosticoActualizaciones});
			this.menuAyuda.Name = "menuAyuda";
			this.menuAyuda.Size = new System.Drawing.Size(53, 20);
			this.menuAyuda.Text = "Ayuda";
			//
			// menuBuscarActualizaciones
			//
			this.menuBuscarActualizaciones.Name = "menuBuscarActualizaciones";
			this.menuBuscarActualizaciones.Size = new System.Drawing.Size(240, 22);
			this.menuBuscarActualizaciones.Text = "Buscar Actualizaciones";
			this.menuBuscarActualizaciones.Click += new System.EventHandler(this.menuBuscarActualizaciones_Click);
			//
			// menuDiagnosticoActualizaciones
			//
			this.menuDiagnosticoActualizaciones.Name = "menuDiagnosticoActualizaciones";
			this.menuDiagnosticoActualizaciones.Size = new System.Drawing.Size(240, 22);
			this.menuDiagnosticoActualizaciones.Text = "Diagnóstico de Actualizaciones";
			this.menuDiagnosticoActualizaciones.Click += new System.EventHandler(this.menuDiagnosticoActualizaciones_Click);
			//
			// menuClickParaActualizar
			//
			this.menuClickParaActualizar.BackColor = System.Drawing.Color.YellowGreen;
			this.menuClickParaActualizar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.menuClickParaActualizar.ForeColor = System.Drawing.Color.DarkGreen;
			this.menuClickParaActualizar.Name = "menuClickParaActualizar";
			this.menuClickParaActualizar.Size = new System.Drawing.Size(144, 20);
			this.menuClickParaActualizar.Text = "Click para Actualizar";
			this.menuClickParaActualizar.Visible = false;
			this.menuClickParaActualizar.Click += new System.EventHandler(this.menuClickParaActualizar_Click);
			//
			// statusStrip1
			//
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblEstado});
			this.statusStrip1.Location = new System.Drawing.Point(0, 428);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(800, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			//
			// lblEstado
			//
			this.lblEstado.Name = "lblEstado";
			this.lblEstado.Size = new System.Drawing.Size(42, 17);
			this.lblEstado.Text = "Listo";
			//
			// timerActualizaciones
			//
			this.timerActualizaciones.Enabled = true;
			this.timerActualizaciones.Interval = 300000;
			this.timerActualizaciones.Tick += new System.EventHandler(this.timerActualizaciones_Tick);
			//
			// btnProbarConexion
			//
			this.btnProbarConexion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
			this.btnProbarConexion.FlatAppearance.BorderSize = 0;
			this.btnProbarConexion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnProbarConexion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			this.btnProbarConexion.ForeColor = System.Drawing.Color.White;
			this.btnProbarConexion.Location = new System.Drawing.Point(30, 80);
			this.btnProbarConexion.Name = "btnProbarConexion";
			this.btnProbarConexion.Size = new System.Drawing.Size(200, 40);
			this.btnProbarConexion.TabIndex = 2;
			this.btnProbarConexion.Text = "Probar Conexión Informix";
			this.btnProbarConexion.UseVisualStyleBackColor = false;
			this.btnProbarConexion.Click += new System.EventHandler(this.btnProbarConexion_Click);
			//
			// txtResultado
			//
			this.txtResultado.Font = new System.Drawing.Font("Consolas", 9F);
			this.txtResultado.Location = new System.Drawing.Point(30, 140);
			this.txtResultado.Multiline = true;
			this.txtResultado.Name = "txtResultado";
			this.txtResultado.ReadOnly = true;
			this.txtResultado.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtResultado.Size = new System.Drawing.Size(740, 260);
			this.txtResultado.TabIndex = 3;
			//
			// lblTitulo
			//
			this.lblTitulo.AutoSize = true;
			this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
			this.lblTitulo.Location = new System.Drawing.Point(30, 40);
			this.lblTitulo.Name = "lblTitulo";
			this.lblTitulo.Size = new System.Drawing.Size(320, 25);
			this.lblTitulo.TabIndex = 4;
			this.lblTitulo.Text = "Prueba de Conexión a Informix";
			//
			// FormPrincipal
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.lblTitulo);
			this.Controls.Add(this.txtResultado);
			this.Controls.Add(this.btnProbarConexion);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormPrincipal";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "CONSOLA";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem menuAyuda;
		private System.Windows.Forms.ToolStripMenuItem menuBuscarActualizaciones;
		private System.Windows.Forms.ToolStripMenuItem menuDiagnosticoActualizaciones;
		private System.Windows.Forms.ToolStripMenuItem menuClickParaActualizar;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel lblEstado;
		private System.Windows.Forms.Timer timerActualizaciones;
		private System.Windows.Forms.Button btnProbarConexion;
		private System.Windows.Forms.TextBox txtResultado;
		private System.Windows.Forms.Label lblTitulo;
	}
}
