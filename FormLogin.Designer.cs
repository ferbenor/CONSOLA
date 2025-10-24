namespace CONSOLA
{
	partial class FormLogin
	{
		/// <summary>
		/// Variable del diseñador necesaria.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Limpiar los recursos que se estén usando.
		/// </summary>
		/// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Código generado por el Diseñador de Windows Forms

		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido de este método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.lblTitulo = new System.Windows.Forms.Label();
			this.lblUsuario = new System.Windows.Forms.Label();
			this.txtUsuario = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.btnEntrar = new System.Windows.Forms.Button();
			this.btnClickParaActualizar = new System.Windows.Forms.Button();
			this.lblEstado = new System.Windows.Forms.Label();
			this.panelHeader = new System.Windows.Forms.Panel();
			this.lblVersion = new System.Windows.Forms.Label();
			this.timerActualizaciones = new System.Windows.Forms.Timer(this.components);
			this.panelHeader.SuspendLayout();
			this.SuspendLayout();
			//
			// lblTitulo
			//
			this.lblTitulo.AutoSize = true;
			this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
			this.lblTitulo.Location = new System.Drawing.Point(100, 80);
			this.lblTitulo.Name = "lblTitulo";
			this.lblTitulo.Size = new System.Drawing.Size(200, 30);
			this.lblTitulo.TabIndex = 0;
			this.lblTitulo.Text = "Iniciar Sesión";
			this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			// lblUsuario
			//
			this.lblUsuario.AutoSize = true;
			this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblUsuario.Location = new System.Drawing.Point(50, 130);
			this.lblUsuario.Name = "lblUsuario";
			this.lblUsuario.Size = new System.Drawing.Size(50, 15);
			this.lblUsuario.TabIndex = 1;
			this.lblUsuario.Text = "Usuario:";
			//
			// txtUsuario
			//
			this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.txtUsuario.Location = new System.Drawing.Point(50, 150);
			this.txtUsuario.Name = "txtUsuario";
			this.txtUsuario.Size = new System.Drawing.Size(300, 25);
			this.txtUsuario.TabIndex = 0;
			//
			// lblPassword
			//
			this.lblPassword.AutoSize = true;
			this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.lblPassword.Location = new System.Drawing.Point(50, 190);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(70, 15);
			this.lblPassword.TabIndex = 3;
			this.lblPassword.Text = "Contraseña:";
			//
			// txtPassword
			//
			this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.txtPassword.Location = new System.Drawing.Point(50, 210);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '●';
			this.txtPassword.Size = new System.Drawing.Size(300, 25);
			this.txtPassword.TabIndex = 1;
			//
			// btnEntrar
			//
			this.btnEntrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
			this.btnEntrar.FlatAppearance.BorderSize = 0;
			this.btnEntrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnEntrar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
			this.btnEntrar.ForeColor = System.Drawing.Color.White;
			this.btnEntrar.Location = new System.Drawing.Point(50, 260);
			this.btnEntrar.Name = "btnEntrar";
			this.btnEntrar.Size = new System.Drawing.Size(300, 40);
			this.btnEntrar.TabIndex = 2;
			this.btnEntrar.Text = "Entrar";
			this.btnEntrar.UseVisualStyleBackColor = false;
			this.btnEntrar.Click += new System.EventHandler(this.btnEntrar_Click);
			//
			// btnClickParaActualizar
			//
			this.btnClickParaActualizar.BackColor = System.Drawing.Color.YellowGreen;
			this.btnClickParaActualizar.FlatAppearance.BorderSize = 0;
			this.btnClickParaActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClickParaActualizar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.btnClickParaActualizar.ForeColor = System.Drawing.Color.DarkGreen;
			this.btnClickParaActualizar.Location = new System.Drawing.Point(50, 310);
			this.btnClickParaActualizar.Name = "btnClickParaActualizar";
			this.btnClickParaActualizar.Size = new System.Drawing.Size(300, 35);
			this.btnClickParaActualizar.TabIndex = 3;
			this.btnClickParaActualizar.Text = "🔄 Click para Actualizar";
			this.btnClickParaActualizar.UseVisualStyleBackColor = false;
			this.btnClickParaActualizar.Visible = false;
			this.btnClickParaActualizar.Click += new System.EventHandler(this.btnClickParaActualizar_Click);
			//
			// lblEstado
			//
			this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
			this.lblEstado.ForeColor = System.Drawing.Color.Gray;
			this.lblEstado.Location = new System.Drawing.Point(50, 355);
			this.lblEstado.Name = "lblEstado";
			this.lblEstado.Size = new System.Drawing.Size(300, 15);
			this.lblEstado.TabIndex = 7;
			this.lblEstado.Text = "Verificando actualizaciones...";
			this.lblEstado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			// panelHeader
			//
			this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
			this.panelHeader.Controls.Add(this.lblVersion);
			this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelHeader.Location = new System.Drawing.Point(0, 0);
			this.panelHeader.Name = "panelHeader";
			this.panelHeader.Size = new System.Drawing.Size(400, 60);
			this.panelHeader.TabIndex = 8;
			//
			// lblVersion
			//
			this.lblVersion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
			this.lblVersion.ForeColor = System.Drawing.Color.White;
			this.lblVersion.Location = new System.Drawing.Point(0, 0);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(400, 60);
			this.lblVersion.TabIndex = 0;
			this.lblVersion.Text = "CONSOLA v1.0.0";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			// timerActualizaciones
			//
			this.timerActualizaciones.Enabled = true;
			this.timerActualizaciones.Interval = 300000;
			this.timerActualizaciones.Tick += new System.EventHandler(this.timerActualizaciones_Tick);
			//
			// FormLogin
			//
			this.AcceptButton = this.btnEntrar;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(400, 400);
			this.Controls.Add(this.panelHeader);
			this.Controls.Add(this.lblEstado);
			this.Controls.Add(this.btnClickParaActualizar);
			this.Controls.Add(this.btnEntrar);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.txtUsuario);
			this.Controls.Add(this.lblUsuario);
			this.Controls.Add(this.lblTitulo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLogin";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Login - CONSOLA";
			this.panelHeader.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label lblTitulo;
		private System.Windows.Forms.Label lblUsuario;
		private System.Windows.Forms.TextBox txtUsuario;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button btnEntrar;
		private System.Windows.Forms.Button btnClickParaActualizar;
		private System.Windows.Forms.Label lblEstado;
		private System.Windows.Forms.Panel panelHeader;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Timer timerActualizaciones;
	}
}
