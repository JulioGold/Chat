using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Net;
using ChatLib;
using Newtonsoft.Json;

namespace ChatCliente
{
    public partial class frmPrincipal : Form
    {
        // Irá conter o nome do usuário
        private string Usuario = "Unknown";
        private static StreamWriter swEnviar;
        private static StreamReader srReceber;
        private static TcpClient tcpServidor;

        private bool RecebendoArquivo = false;
        private string PathArquivo = "";

        // Necessário para atualizar o form com mensagens vindas de outra thread
        private delegate void UpdateLogCallback(string strMessage);

        // Necessário para definir o estado do form como "desconectado" vindo de outra thread
        private delegate void CloseConnectionCallback(string strReason);

        // Necessário para atualizar a lista de usuários do form com o que veio de outra thread
        private delegate void UpdateListaUsuariosCallback(Hashtable listaUsuarios);

        // Thread que irá cuidar do gerenciamento das mensagens recebidas e enviadas
        private Thread threadMensagens;

        private IPAddress endereçoIP;
        private static bool Conectado;

        private Pacote pacoteRecebido;
        private Pacote pacoteEnviar;

        public frmPrincipal()
        {
            // Não esquecer de fechar os objetos primeiro quando for fechar a aplicação
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();
        }

        // Manipulador do evento quando sair da aplicação
        public void OnApplicationExit(object sender, EventArgs e)
        {
            if (Conectado == true)
            {
                // Fecha os objetos
                Conectado = false;
                swEnviar.Close();
                srReceber.Close();
                tcpServidor.Close();
            }
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            // Se não está conectado mas aguardando para conectar
            if (Conectado == false)
            {
                // Inicia a conexão
                IniciaConexao();
            }
            else // Está conectado, então vou desconectar
            {
                FinalizaConexao("Usuário se desconectou.");
            }
        }

        // Inicia a conexão
        private void IniciaConexao()
        {
            endereçoIP = IPAddress.Parse(txtIp.Text);

            // Inicia a conexão com o servidor do chat
            tcpServidor = new TcpClient();
            tcpServidor.Connect(endereçoIP, (int)nudPorta.Value);

            // Define que está conectado
            Conectado = true;

            Usuario = txtUsuario.Text;

            // Desabilita e habilita os devidos campos
            txtIp.Enabled = false;
            txtUsuario.Enabled = false;
            nudPorta.Enabled = false;
            txtMensagem.Enabled = true;
            btnEnviar.Enabled = true;
            btnConectar.Text = "Desconectar";

            // Stream para envio de pacotes para o servidor
            swEnviar = new StreamWriter(tcpServidor.GetStream());

            // Criando novo pacote
            pacoteEnviar = new Pacote();
            pacoteEnviar.Comando = Comando.Conectar;
            pacoteEnviar.Usuario = this.Usuario;
            pacoteEnviar.Mensagem = "entrando no chat...";
            string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
            swEnviar.WriteLine(objEnviar);
            swEnviar.Flush();

            // Inicia a thread para receber pacotes do servidor e novas comunicações do mesmo
            threadMensagens = new Thread(new ThreadStart(RecebeMensagens));
            threadMensagens.SetApartmentState(ApartmentState.STA);
            threadMensagens.Start();
        }


        // Fecha a conexão atual
        private void FinalizaConexao(string motivo)
        {
            txtLog.AppendText(motivo + "\r\n");
            txtIp.Enabled = true;
            nudPorta.Enabled = true;
            txtUsuario.Enabled = true;
            txtMensagem.Enabled = false;
            btnEnviar.Enabled = false;
            btnConectar.Text = "Conectar";
            lbUsuarios.Items.Clear();

            // Fecha os objetos
            Conectado = false;
            swEnviar.Close();
            srReceber.Close();
            tcpServidor.Close();
        }

        // Responsável por gerenciar as mensagens
        private void RecebeMensagens()
        {
            // Recebe os pacotes do servidor
            srReceber = new StreamReader(tcpServidor.GetStream());

            // Lê o pacote recebido
            string retorno = srReceber.ReadLine();

            pacoteRecebido = JsonConvert.DeserializeObject<Pacote>(retorno);

            // Conforme o comando recebido vejo o que aconteceu
            if (pacoteRecebido.Comando == Comando.ConectadoComSucesso)
            {
                // Atualiza o form para informar que se conectou
                this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { "Conectado com sucesso!" });
            }
            else // Como o comando não foi de conectado, então qualquer outro comando neste momento é porque não se conectou
            {
                string motivo = "Não conectado: " + pacoteRecebido.Mensagem;

                // Atualiza o form com o motivo de porque não pode se conectar
                this.Invoke(new CloseConnectionCallback(this.FinalizaConexao), new object[] { motivo });

                // Sai do método
                return;
            }

            // Enquanto estiver conectado lê as linhas que vierem do servidor
            while (Conectado)
            {
                try
                {
                    string conteudoRecebido = srReceber.ReadLine();

                    if (conteudoRecebido != null)
                    {
                        pacoteRecebido = JsonConvert.DeserializeObject<Pacote>(conteudoRecebido);

                        if (pacoteRecebido.Comando == Comando.ListaDeUsuarios)
                        {
                            this.Invoke(new UpdateListaUsuariosCallback(this.UpdateLista), new object[] { pacoteRecebido.ListaDeUsuarios });
                        }
                        else if (pacoteRecebido.Comando == Comando.EnviarArquivo ||
                                pacoteRecebido.Comando == Comando.AceiteDoArquivo ||
                                pacoteRecebido.Comando == Comando.ArquivoRecusado ||
                                pacoteRecebido.Comando == Comando.EnviandoArquivo)
                        {
                            // Estão querendo me enviar um arquivo
                            if (pacoteRecebido.Comando == Comando.EnviarArquivo && RecebendoArquivo == false)
                            {
                                if (MessageBox.Show(this.Usuario + " você deseja receber o arquivo: " + pacoteRecebido.NomeArquivo + " de " + pacoteRecebido.Usuario + "?", "Receber arquivo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    RecebendoArquivo = true;
                                    // Criando novo pacote
                                    pacoteEnviar = new Pacote();
                                    pacoteEnviar.Comando = Comando.AceiteDoArquivo;
                                    pacoteEnviar.Usuario = this.Usuario;
                                    pacoteEnviar.Mensagem = pacoteRecebido.Mensagem;
                                    pacoteEnviar.NomeArquivo = pacoteRecebido.NomeArquivo;
                                    pacoteEnviar.TamanhoArquivo = pacoteRecebido.TamanhoArquivo;
                                    pacoteEnviar.ConteudoArquivo = null;
                                    pacoteEnviar.UsuarioDestino = pacoteRecebido.Usuario;
                                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                                    swEnviar.WriteLine(objEnviar);
                                    swEnviar.Flush();
                                }
                            }
                            else if (pacoteRecebido.Comando == Comando.AceiteDoArquivo) // Aceitou o arquivo, vou enviar ele agora
                            {
                                // Lê o arquivo de forma binária
                                FileStream fsr = new FileStream(PathArquivo, FileMode.Open);
                                byte[] bufferRead = new byte[fsr.Length];
                                BinaryReader br = new BinaryReader(fsr);
                                br.Read(bufferRead, 0, (int)fsr.Length);
                                br.Close();

                                // Converte o arquivo em base64 para poder enviar como string
                                string arquivoConvertido = Convert.ToBase64String(bufferRead);

                                // Criando novo pacote
                                pacoteEnviar = new Pacote();
                                pacoteEnviar.Comando = Comando.EnviandoArquivo;
                                pacoteEnviar.Usuario = this.Usuario;
                                pacoteEnviar.Mensagem = null;
                                pacoteEnviar.UsuarioDestino = pacoteRecebido.Usuario;
                                pacoteEnviar.NomeArquivo = Path.GetFileName(this.PathArquivo);
                                pacoteEnviar.TamanhoArquivo = bufferRead.LongLength.ToString();
                                pacoteEnviar.ConteudoArquivo = arquivoConvertido;
                                string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                                swEnviar.WriteLine(objEnviar);
                                swEnviar.Flush();
                            }
                            else if (pacoteRecebido.Comando == Comando.EnviandoArquivo && RecebendoArquivo) // O arquivo foi aceito e estou recebendo ele agora
                            {
                                SaveFileDialog sfd = new SaveFileDialog();
                                sfd.RestoreDirectory = true;
                                sfd.FileName = pacoteRecebido.NomeArquivo;
                                if (sfd.ShowDialog() == DialogResult.OK)
                                {
                                    byte[] bufferFromBase4 = Convert.FromBase64String(pacoteRecebido.ConteudoArquivo);
                                    FileStream fsw2 = new FileStream(sfd.FileName, FileMode.Create);
                                    BinaryWriter bw2 = new BinaryWriter(fsw2);
                                    bw2.Write(bufferFromBase4);
                                    bw2.Close();
                                    RecebendoArquivo = false;
                                }
                            }
                            else
                            {
                                // Criando novo pacote
                                pacoteEnviar = new Pacote();
                                pacoteEnviar.Comando = Comando.ArquivoRecusado;
                                pacoteEnviar.Usuario = this.Usuario;
                                pacoteEnviar.UsuarioDestino = pacoteRecebido.Usuario;
                                pacoteEnviar.Mensagem = pacoteRecebido.Mensagem;
                                pacoteEnviar.NomeArquivo = pacoteRecebido.NomeArquivo;
                                pacoteEnviar.TamanhoArquivo = pacoteRecebido.TamanhoArquivo;
                                pacoteEnviar.ConteudoArquivo = null;
                                string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                                swEnviar.WriteLine(objEnviar);
                                swEnviar.Flush();
                            }
                        }
                        else
                        {
                            this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { pacoteRecebido.Mensagem });
                        }
                    }
                }
                catch (SocketException SocketEx)
                {
                    Conectado = false;
                    //throw SocketEx;
                }
                catch (Exception ex)
                {
                    Conectado = false;
                    //throw ex;
                }
            }
        }

        // Este método é chamado de uma thread diferente e serve para atualizar o textbox de log do chat
        private void UpdateLog(Pacote pacote)
        {
            // Adiciona a linha e rola o texto para baixo
            txtLog.AppendText(pacote.Mensagem + "\r\n");
        }


        // Este método é chamado de uma thread diferente e serve para atualizar o textbox de log do chat
        private void UpdateLog(string mensagem)
        {
            // Adiciona a linha e rola o texto para baixo
            txtLog.AppendText(mensagem + "\r\n");
        }


        // Atualiza a lista de usuários
        private void UpdateLista(Hashtable listaUsuarios)
        {
            lbUsuarios.Items.Clear();
            foreach (DictionaryEntry entry in listaUsuarios)
            {
                // A chave da Hashtable é o próprio nome do usuário
                lbUsuarios.Items.Add(entry.Key);
            }
        }


        // Envia a mensagem digitada para o servidor
        private void EnviaMensagem()
        {
            if (txtMensagem.Lines.Length >= 1)
            {
                if (lbUsuarios.Text != null && lbUsuarios.Text != "" && lbUsuarios.Text != this.Usuario)
                {
                    pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.MensagemPrivada;
                    pacoteEnviar.Usuario = this.Usuario;
                    pacoteEnviar.UsuarioDestino = lbUsuarios.Text;
                    pacoteEnviar.Mensagem = txtMensagem.Text;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    //var resposta = Encoding.ASCII.GetBytes(objEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    txtMensagem.Lines = null;
                }
                else
                {
                    pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.Mensagem;
                    pacoteEnviar.Usuario = this.Usuario;
                    pacoteEnviar.Mensagem = txtMensagem.Text;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    //var resposta = Encoding.ASCII.GetBytes(objEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    txtMensagem.Lines = null;
                }
            }
            txtMensagem.Text = "";
        }


        private void btnEnviar_Click(object sender, EventArgs e)
        {
            EnviaMensagem();
        }


        // Queremos que a mensagem seja enviada ao teclar o Enter
        private void txtMensagem_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Se a tecla é o Enter
            if (e.KeyChar == (char)13)
            {
                EnviaMensagem();
            }
        }


        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = new ToolStripMenuItem("Enviar arquivo");
            tsmi.Click += new EventHandler(tsmi_Click);
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add(tsmi);
            cms.Opening += new CancelEventHandler(cms_Opening);
            lbUsuarios.ContextMenuStrip = cms;
        }


        void cms_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip obj = (ContextMenuStrip)sender;
            // Se o usuário selecionado for diferente de vazio e se não é o próprio usuário
            if (lbUsuarios.Text != String.Empty && lbUsuarios.Text != this.Usuario)
            {
                obj.Enabled = true;
            }
            else
            {
                obj.Enabled = false;
            }
        }


        private void tsmi_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Guarda o path do arquivo
                PathArquivo = ofd.FileName;

                FileInfo fi = new FileInfo(ofd.FileName);

                // Enviando para o Servidor->UsuarioDestino a mensagem dizendo que eu quero enviar um arquivo
                pacoteEnviar = new Pacote();
                pacoteEnviar.Comando = Comando.EnviarArquivo;
                pacoteEnviar.Usuario = this.Usuario;
                pacoteEnviar.UsuarioDestino = lbUsuarios.Text;
                pacoteEnviar.Mensagem = null;
                pacoteEnviar.NomeArquivo = Path.GetFileName(ofd.FileName);
                pacoteEnviar.TamanhoArquivo = fi.Length.ToString();
                pacoteEnviar.ConteudoArquivo = null;
                string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                swEnviar.WriteLine(objEnviar);
                swEnviar.Flush();
                txtMensagem.Lines = null;
            }
        }

        private void lbUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.Text != "" && lb.Text != Usuario)
            {
                this.tsslMensagem.Text = "Enviar mensagem reservada para: " + lb.Text;
            }
            else
            {
                this.tsslMensagem.Text = "Mensagem para todos";
            }
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSobre formSobre = new frmSobre();
            formSobre.Show();
        }

    }
}
