using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using ChatLib;
using Newtonsoft.Json;

namespace ChatServidor
{
    // Esta classe manipula as conexões; Serão várias instâncias desta conforme a quantidade de usuários conectados.
    class Conexao
    {
        TcpClient tcpCliente;

        // Esta thread que irá enviar informações para o cliente
        private Thread thrSender;

        private StreamReader srRecebe = null;
        private StreamWriter swEnvia = null;

        // Nome do usuário desta conexão
        private string UsuarioConexao;
        private string strResposta;

        private Pacote pacoteRecebido;
        private Pacote pacoteEnviar;

        #region Construtores
        // O construtor da classe recebe uma conexão TCP.
        public Conexao(TcpClient tcpCon)
        {
            tcpCliente = tcpCon;

            // Instancia a thread que irá receber o cliente e aguarda por mensagens
            thrSender = new Thread(AceitaCliente);

            // A própria thread chama o método AceitaCliente()
            thrSender.Start();
        }


        // O construtor da classe recebe uma conexão TCP e um pacote.
        public Conexao(TcpClient tcpCon, Pacote pacoteRecebido)
        {
            this.tcpCliente = tcpCon;

            this.pacoteRecebido = pacoteRecebido;

            // Instancia a thread que irá receber o cliente e aguarda por mensagens
            thrSender = new Thread(AceitaCliente);

            // A própria thread chama o método AceitaCliente()
            thrSender.Start();
        }
        #endregion

        private void FechaConexao()
        {
            // Fecha os objetos atualmente abertos
            tcpCliente.Close();
            this.srRecebe.Close();
            this.swEnvia.Close();
        }

        // Ocorre quando o novo cliente é aceito
        private void AceitaCliente()
        {
            this.srRecebe = new StreamReader(tcpCliente.GetStream());
            this.swEnvia = new StreamWriter(tcpCliente.GetStream());

            // Se o pacote ja existe e o comando dele é de Conectar então não preciso fazer nada, do contrário devo ler a informação que vier
            if (this.pacoteRecebido != null && this.pacoteRecebido.Comando == Comando.Conectar)
            {

            }
            else
            {
                // Lê as informações de conta que vieram do cliente
                string conteudoRecebido = this.srRecebe.ReadLine();

                // Limpo o pacote atual, caso dê alguma treta não vai seguir adiante com o pacote velho
                this.pacoteRecebido = null;

                if (conteudoRecebido != null)
                {
                    this.pacoteRecebido = JsonConvert.DeserializeObject<Pacote>(conteudoRecebido);
                }
            }

            // Se tem pacote vou trabalhar com ele
            if (this.pacoteRecebido != null)
            {
                this.UsuarioConexao = pacoteRecebido.Usuario;

                Pacote pacoteEnviar = new Pacote();
                pacoteEnviar.Usuario = "Administrator";

                // Se é um pacote para parar a escuta, então paro!
                if (pacoteRecebido.Comando == Comando.PararEscuta)
                {
                    FechaConexao();
                    return;
                }
                else if (this.UsuarioConexao != "" && pacoteRecebido.Comando == Comando.Conectar) // Se esta conexão não tem usuário e o comando é pedindo para conectar...
                {
                    // Verifica se o nome do usuário ja não existe na lista de usuários
                    if (ChatServer.htUsuarios.Contains(this.UsuarioConexao) == true)
                    {
                        // Comando é que este nome ja está sendo utilizado
                        pacoteEnviar.Comando = Comando.NomeReservado;
                        pacoteEnviar.Mensagem = "Este nome ja está sendo utilizado.";
                        string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                        this.swEnvia.WriteLine(objEnviar);
                        this.swEnvia.Flush();
                        FechaConexao();
                        return;
                    }
                    // Se o nome do usuário malandro é igual ao nome do administrador
                    else if (this.UsuarioConexao == "Administrator")
                    {
                        // Comando é que este nome de usuário não pode ser utilizado
                        pacoteEnviar.Comando = Comando.NomeReservado;
                        pacoteEnviar.Mensagem = "Este nome é reservado.";
                        string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                        this.swEnvia.WriteLine(objEnviar);
                        this.swEnvia.Flush();
                        FechaConexao();
                        return;
                    }
                    else
                    {
                        // Comando é que foi conectado com sucesso
                        pacoteEnviar.Comando = Comando.ConectadoComSucesso;
                        pacoteEnviar.Mensagem = "";
                        string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                        this.swEnvia.WriteLine(objEnviar);
                        this.swEnvia.Flush();

                        // Adiciona o usuário na lista e começa a escutar as mensagem vindas dele
                        ChatServer.AdicionaUsuario(tcpCliente, this.UsuarioConexao);
                    }
                }
                else
                {
                    FechaConexao();
                    return;
                }

                try
                {
                    // Continua aguardando por uma mensagem enviada pelo usuário
                    while ((strResposta = this.srRecebe.ReadLine()) != "")
                    {
                        // Se o conteúdo enviado e null então removo o usupário da lista
                        if (strResposta == null)
                        {
                            ChatServer.RemoveUsuario(tcpCliente);
                        }
                        else
                        {
                            // Agora vou tratar as próximas requisições
                            pacoteRecebido = JsonConvert.DeserializeObject<Pacote>(strResposta);

                            switch (pacoteRecebido.Comando)
                            {
                                case Comando.NomeReservado:
                                    // Ele está de brincadeira com este comando, vou remover este engraçadinho
                                    ChatServer.RemoveUsuario(tcpCliente);
                                    break;

                                case Comando.ConectadoComSucesso:
                                    // Ele está de brincadeira com este comando, vou remover este engraçadinho
                                    ChatServer.RemoveUsuario(tcpCliente);
                                    break;

                                case Comando.ListaDeUsuarios:
                                    // Ele está de brincadeira com este comando, vou remover este engraçadinho
                                    ChatServer.RemoveUsuario(tcpCliente);
                                    break;

                                case Comando.Mensagem:
                                    // Envia mensagem normal para todos os outros usuários
                                    ChatServer.EnviaMensagem(pacoteRecebido);
                                    break;

                                case Comando.MensagemPrivada:
                                    // Envia a mensagem apenas para o usuário determinado
                                    ChatServer.EnviaMensagemPrivada(pacoteRecebido);
                                    break;

                                case Comando.EnviarArquivo:
                                    // Envia a mensagem dizendo que um usuário deseja enviar um arquivo para o outro
                                    ChatServer.EnviarArquivo(pacoteRecebido);
                                    break;

                                case Comando.AceiteDoArquivo:
                                    ChatServer.AceiteDoArquivo(pacoteRecebido);
                                    break;

                                case Comando.EnviandoArquivo:
                                    ChatServer.EnviandoArquivo(pacoteRecebido);
                                    break;

                                case Comando.ArquivoRecusado:
                                    ChatServer.ArquivoRecusado(pacoteRecebido);
                                    break;

                                case Comando.Conectar:
                                    // Ele está de brincadeira com este comando, vou remover este engraçadinho
                                    ChatServer.RemoveUsuario(tcpCliente);
                                    break;

                                default:
                                    // Ele está de brincadeira, vou remover este engraçadinho
                                    ChatServer.RemoveUsuario(tcpCliente);
                                    break;
                            }
                        }
                    }
                }
                catch
                {
                    // Se algo está errado com este usuário, disconecto o mesmo.
                    ChatServer.RemoveUsuario(tcpCliente);
                }
            }

        }
    }
}
