using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Net;
using ChatLib;

namespace ChatServidor
{
    public partial class frmPrincipal : Form
    {
        // Delegates que serão encarregados de executar as atualizações quando ocorrigo algo
        private delegate void UpdateStatusCallback(string strMessage);
        private delegate void UpdateListaUsuariosCallback(Hashtable listaUsuarios);


        public frmPrincipal()
        {
            // Não esquecer de fechar os objetos primeiro quando for fechar a aplicação
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();
        }


        // Manipulador do evento quando sair da aplicação
        public void OnApplicationExit(object sender, EventArgs e)
        {
            //servidorPrincipal.ParaAEscuta();
        }


        public void mainServer_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            try
            {
                // Chama o método que é encarregado de atualizar o form
                this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { e.EventMessage });
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }


        public void mainServer_ListaChanged(object sender, ListaChangedEventArgs e)
        {
            // Chama o método encarregado de enviar a lista de usuários atualizada para os usuários conectados
            this.Invoke(new UpdateListaUsuariosCallback(this.UpdateLista), new object[] { e.EventListaUsuarios });
        }


        private void UpdateStatus(string mensagem)
        {
            // Atualiza o log com a mensagem
            txtLog.AppendText(mensagem + "\r\n");
        }


        private void UpdateLista(Hashtable listaUsuarios)
        {
            lbUsuarios.Items.Clear();
            foreach (DictionaryEntry entry in listaUsuarios)
            {
                // A chave da Hashtable é o próprio nome do usuário
                lbUsuarios.Items.Add(entry.Key);
            }
        }


        private void btnIniciar_Click(object sender, EventArgs e)
        {
            // Cria a instância para o objeto ChatServer
            ChatServer servidorPrincipal = new ChatServer(IPAddress.Parse(txtIp.Text));

            // Gancho para o manipulador de evento StatusChange atribuído ao método mainServer_StatusChanged
            ChatServer.StatusChanged += new StatusChangedEventHandler(mainServer_StatusChanged);

            // Gancho para o manipulador de evento ListaChanged atribuído ao método mainServer_ListaChanged
            ChatServer.ListaChanged += new ListaChangedEventHandler(mainServer_ListaChanged);

            Button obj = (Button)sender;
            if (obj.Text == "Parar servidor")
            {
                // Quando está rodando e vai parar

                obj.Text = "Iniciar servidor";
                txtIp.Enabled = true;
                nudPorta.Enabled = true;

                // Para de escutar por novas conexões
                servidorPrincipal.ParaAEscuta();

                // Limpa a lista de usuários
                lbUsuarios.Items.Clear();

                // Exibe que parou de escutar por novas conexões
                txtLog.AppendText("Monitoramento de conexões parado.\r\n");
            }
            else
            {
                // Quando está parado e vai rodar

                obj.Text = "Parar servidor";
                txtIp.Enabled = false;
                nudPorta.Enabled = false;

                // Inicia a escuta por novas conexões
                servidorPrincipal.IniciaAEscuta();

                // Exibe que iniciou o monitoramento de conexões
                txtLog.AppendText("Monitorando conexões...\r\n");
            }
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSobre formSobre = new frmSobre();
            formSobre.Show();
        }
    }
}
