namespace ChatCliente
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblServidor = new System.Windows.Forms.Label();
            this.lblPorta = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.nudPorta = new System.Windows.Forms.NumericUpDown();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.btnConectar = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lbUsuarios = new System.Windows.Forms.ListBox();
            this.txtMensagem = new System.Windows.Forms.TextBox();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.ssRodape = new System.Windows.Forms.StatusStrip();
            this.tsslMensagem = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.sobreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.nudPorta)).BeginInit();
            this.ssRodape.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblServidor
            // 
            this.lblServidor.AutoSize = true;
            this.lblServidor.Location = new System.Drawing.Point(20, 46);
            this.lblServidor.Name = "lblServidor";
            this.lblServidor.Size = new System.Drawing.Size(60, 13);
            this.lblServidor.TabIndex = 0;
            this.lblServidor.Text = "IP servidor:";
            // 
            // lblPorta
            // 
            this.lblPorta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPorta.AutoSize = true;
            this.lblPorta.Location = new System.Drawing.Point(315, 46);
            this.lblPorta.Name = "lblPorta";
            this.lblPorta.Size = new System.Drawing.Size(35, 13);
            this.lblPorta.TabIndex = 1;
            this.lblPorta.Text = "Porta:";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(20, 72);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(46, 13);
            this.lblUsuario.TabIndex = 2;
            this.lblUsuario.Text = "Usuário:";
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(86, 43);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(128, 20);
            this.txtIp.TabIndex = 3;
            this.txtIp.Text = "127.0.0.1";
            // 
            // nudPorta
            // 
            this.nudPorta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPorta.Location = new System.Drawing.Point(362, 44);
            this.nudPorta.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.nudPorta.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPorta.Name = "nudPorta";
            this.nudPorta.Size = new System.Drawing.Size(120, 20);
            this.nudPorta.TabIndex = 4;
            this.nudPorta.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(86, 69);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(128, 20);
            this.txtUsuario.TabIndex = 5;
            this.txtUsuario.Text = "Julio";
            // 
            // btnConectar
            // 
            this.btnConectar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConectar.Location = new System.Drawing.Point(362, 67);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(120, 23);
            this.btnConectar.TabIndex = 6;
            this.btnConectar.Text = "Conectar";
            this.btnConectar.UseVisualStyleBackColor = true;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(23, 107);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(275, 303);
            this.txtLog.TabIndex = 7;
            // 
            // lbUsuarios
            // 
            this.lbUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbUsuarios.FormattingEnabled = true;
            this.lbUsuarios.Location = new System.Drawing.Point(304, 107);
            this.lbUsuarios.Name = "lbUsuarios";
            this.lbUsuarios.Size = new System.Drawing.Size(178, 303);
            this.lbUsuarios.TabIndex = 8;
            this.lbUsuarios.SelectedIndexChanged += new System.EventHandler(this.lbUsuarios_SelectedIndexChanged);
            // 
            // txtMensagem
            // 
            this.txtMensagem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMensagem.Location = new System.Drawing.Point(23, 418);
            this.txtMensagem.Name = "txtMensagem";
            this.txtMensagem.Size = new System.Drawing.Size(378, 20);
            this.txtMensagem.TabIndex = 9;
            this.txtMensagem.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMensagem_KeyPress);
            // 
            // btnEnviar
            // 
            this.btnEnviar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnviar.Location = new System.Drawing.Point(407, 416);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(75, 23);
            this.btnEnviar.TabIndex = 10;
            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.UseVisualStyleBackColor = true;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);
            // 
            // ssRodape
            // 
            this.ssRodape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslMensagem,
            this.toolStripStatusLabel1});
            this.ssRodape.Location = new System.Drawing.Point(0, 450);
            this.ssRodape.Name = "ssRodape";
            this.ssRodape.Size = new System.Drawing.Size(503, 22);
            this.ssRodape.TabIndex = 11;
            this.ssRodape.Text = "statusStrip1";
            // 
            // tsslMensagem
            // 
            this.tsslMensagem.Name = "tsslMensagem";
            this.tsslMensagem.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(125, 17);
            this.toolStripStatusLabel1.Text = "Mensagem para todos";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sobreToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(503, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "msMenu";
            // 
            // sobreToolStripMenuItem
            // 
            this.sobreToolStripMenuItem.Name = "sobreToolStripMenuItem";
            this.sobreToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.sobreToolStripMenuItem.Text = "Sobre...";
            this.sobreToolStripMenuItem.Click += new System.EventHandler(this.sobreToolStripMenuItem_Click);
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 472);
            this.Controls.Add(this.ssRodape);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.txtMensagem);
            this.Controls.Add(this.lbUsuarios);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnConectar);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.nudPorta);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.lblPorta);
            this.Controls.Add(this.lblServidor);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmPrincipal";
            this.Text = "Chat cliente";
            this.Load += new System.EventHandler(this.frmPrincipal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudPorta)).EndInit();
            this.ssRodape.ResumeLayout(false);
            this.ssRodape.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblServidor;
        private System.Windows.Forms.Label lblPorta;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.NumericUpDown nudPorta;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Button btnConectar;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ListBox lbUsuarios;
        private System.Windows.Forms.TextBox txtMensagem;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.StatusStrip ssRodape;
        private System.Windows.Forms.ToolStripStatusLabel tsslMensagem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sobreToolStripMenuItem;
    }
}

