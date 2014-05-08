namespace ChatServidor
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
            this.lblIp = new System.Windows.Forms.Label();
            this.lblPorta = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.nudPorta = new System.Windows.Forms.NumericUpDown();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lbUsuarios = new System.Windows.Forms.ListBox();
            this.msMenu = new System.Windows.Forms.MenuStrip();
            this.sobreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.nudPorta)).BeginInit();
            this.msMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblIp
            // 
            this.lblIp.AutoSize = true;
            this.lblIp.Location = new System.Drawing.Point(12, 47);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(75, 13);
            this.lblIp.TabIndex = 0;
            this.lblIp.Text = "IP do servidor:";
            // 
            // lblPorta
            // 
            this.lblPorta.AutoSize = true;
            this.lblPorta.Location = new System.Drawing.Point(227, 47);
            this.lblPorta.Name = "lblPorta";
            this.lblPorta.Size = new System.Drawing.Size(35, 13);
            this.lblPorta.TabIndex = 1;
            this.lblPorta.Text = "Porta:";
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(93, 44);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(100, 20);
            this.txtIp.TabIndex = 2;
            this.txtIp.Text = "127.0.0.1";
            // 
            // nudPorta
            // 
            this.nudPorta.Location = new System.Drawing.Point(268, 45);
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
            this.nudPorta.Size = new System.Drawing.Size(101, 20);
            this.nudPorta.TabIndex = 3;
            this.nudPorta.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // btnIniciar
            // 
            this.btnIniciar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIniciar.Location = new System.Drawing.Point(546, 42);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(100, 23);
            this.btnIniciar.TabIndex = 4;
            this.btnIniciar.Text = "Iniciar servidor";
            this.btnIniciar.UseVisualStyleBackColor = true;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(199, 71);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(447, 444);
            this.txtLog.TabIndex = 5;
            // 
            // lbUsuarios
            // 
            this.lbUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbUsuarios.FormattingEnabled = true;
            this.lbUsuarios.Location = new System.Drawing.Point(15, 70);
            this.lbUsuarios.Name = "lbUsuarios";
            this.lbUsuarios.Size = new System.Drawing.Size(178, 446);
            this.lbUsuarios.TabIndex = 6;
            // 
            // msMenu
            // 
            this.msMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sobreToolStripMenuItem});
            this.msMenu.Location = new System.Drawing.Point(0, 0);
            this.msMenu.Name = "msMenu";
            this.msMenu.Size = new System.Drawing.Size(658, 24);
            this.msMenu.TabIndex = 7;
            this.msMenu.Text = "menuStrip1";
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
            this.ClientSize = new System.Drawing.Size(658, 527);
            this.Controls.Add(this.lbUsuarios);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnIniciar);
            this.Controls.Add(this.nudPorta);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.lblPorta);
            this.Controls.Add(this.lblIp);
            this.Controls.Add(this.msMenu);
            this.MainMenuStrip = this.msMenu;
            this.Name = "frmPrincipal";
            this.Text = "Chat Servidor";
            ((System.ComponentModel.ISupportInitialize)(this.nudPorta)).EndInit();
            this.msMenu.ResumeLayout(false);
            this.msMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.Label lblPorta;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.NumericUpDown nudPorta;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ListBox lbUsuarios;
        private System.Windows.Forms.MenuStrip msMenu;
        private System.Windows.Forms.ToolStripMenuItem sobreToolStripMenuItem;
    }
}

