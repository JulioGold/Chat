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
    #region Eventos
    // Contém os argumentos para o evento StatusChanged
    public class StatusChangedEventArgs : EventArgs
    {
        // O argumento, estamos interessados em uma mensagem descrevendo o evento
        private string EventMsg;

        // Propriedade para recuperação e ajuste do evento de mensagem
        public string EventMessage
        {
            get
            {
                return EventMsg;
            }
            set
            {
                EventMsg = value;
            }
        }

        // Construtor para configurar o evento message
        public StatusChangedEventArgs(string strEventMsg)
        {
            EventMsg = strEventMsg;
        }
    }

    // Este delegate é necessário para especificar os parâmetros que estamos passando com o nosso evento
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);

    /*--------------------------------------------------------------------------------------------------*/

    // Contém os argumentos para o evento StatusChanged
    public class ListaChangedEventArgs : EventArgs
    {
        // O argumento, estamos interessados em uma mensagem descrevendo o evento
        private Hashtable htUsuarios;

        // Propriedade para recuperação e ajuste do evento de mensagem
        public Hashtable EventListaUsuarios
        {
            get
            {
                return htUsuarios;
            }
            set
            {
                htUsuarios = value;
            }
        }

        // Construtor para configurar o evento message
        public ListaChangedEventArgs(Hashtable listaUsuarios)
        {
            htUsuarios = listaUsuarios;
        }
    }

    // Este delegate é necessário para especificar os parâmetros que estamos passando com o nosso evento
    public delegate void ListaChangedEventHandler(object sender, ListaChangedEventArgs e);
    #endregion

    class ChatServer
    {
        // Esta hash table armazena os usuários e conexões (chave por nome de usuário)
        public static Hashtable htUsuarios = new Hashtable();

        // Esta hash table armazena os usuários e conexões (chave por conexão)
        public static Hashtable htConexoes = new Hashtable();

        // Guarda o endereço IP que o servidor irá utilizar
        private IPAddress EnderecoIP;
        private int porta = 80;

        // Identificador do administrador
        private static string IDAdministrador = Guid.NewGuid().ToString();

        // Nome de usuário do usuário que é administrador
        private static string NomeAdministrador = "Administrador";

        private TcpClient tcpCliente;

        // O evento e seu argumento que irá notificar o form quando um usuário tiver conectado, desconectado, enviado mensagem...
        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;

        // O evento e seu argumento que irá enviar para os demais usuários a lista de usuários atualizada
        public static event ListaChangedEventHandler ListaChanged;
        private static ListaChangedEventArgs Listae;

        #region Construtores
        // O construtor define o endereço IP para escutar
        public ChatServer(IPAddress enderecoIP)
        {
            EnderecoIP = enderecoIP;
        }

        // O construtor define o endereço IP para escutar e também qual a porta
        public ChatServer(IPAddress enderecoIP, int porta)
        {
            this.EnderecoIP = enderecoIP;
            this.porta = porta;
        }
        #endregion

        // A thread que irá cuidar do listener de conexão
        private Thread thrListener;

        // Este objeto é quem irá escutar as conexões
        private static TcpListener tlsCliente;

        // Isto é para controlar o loop que monitora por novas conexões
        bool ServidorRodando = false;


        // Adiciona o usuário nas tabelas hash
        public static void AdicionaUsuario(TcpClient tcpUsuario, string NomeUsuario)
        {
            //Primeiro adiciona o nome do usuário e associa a conexão, isso em ambas as tabelas hash
            ChatServer.htUsuarios.Add(NomeUsuario, tcpUsuario);
            ChatServer.htConexoes.Add(tcpUsuario, NomeUsuario);

            // Avisa que a lista de usuários mudou
            Listae = new ListaChangedEventArgs(ChatServer.htUsuarios);
            OnListaChanged(Listae);

            // Envia a lista de usuários atualizada para todos que estão conectados
            EnviaListaDeUsuariosAtualizada();

            // Avisa sobre a nova conexão para todos os usuários e para o form do servidor também
            EnviaMensagemDeAdministrador(htConexoes[tcpUsuario] + " entrou no chat");
        }


        // Remove o usuário das tabelas hash
        public static void RemoveUsuario(TcpClient tcpUsuario)
        {
            // Se o usuário está na lista de conexões
            if (htConexoes[tcpUsuario] != null)
            {
                // Primeiro mostra a informação na tela e fala para os outros usuários que este usuário se desconectou
                EnviaMensagemDeAdministrador(htConexoes[tcpUsuario] + " saiu do chat");

                // Remove o usuário das tabelas hash
                ChatServer.htUsuarios.Remove(ChatServer.htConexoes[tcpUsuario]);
                ChatServer.htConexoes.Remove(tcpUsuario);

                // Exibe na aplicação quem disse o que
                Listae = new ListaChangedEventArgs(ChatServer.htUsuarios);
                OnListaChanged(Listae);

                // Envia a lista de usuários atualizada para todos que estão conectados
                EnviaListaDeUsuariosAtualizada();
            }
        }


        // É chamado quando o evento StatusChanged ocorre
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                // Invoke the delegate
                statusHandler(null, e);
            }
        }


        // É chamado quando o evento ListaChanged ocorre
        public static void OnListaChanged(ListaChangedEventArgs e)
        {
            ListaChangedEventHandler statusHandler = ListaChanged;
            if (statusHandler != null)
            {
                // Invoke the delegate
                statusHandler(null, e);
            }
        }


        // Envia mensagens administrativas
        public static void EnviaMensagemDeAdministrador(string mensagem)
        {
            // Primeiro de tudo, exibe na aplicação quem diz o que
            e = new StatusChangedEventArgs(ChatServer.NomeAdministrador + ": " + mensagem);
            OnStatusChanged(e);

            // Cria um array de clientes TCP, o tamanho é o número de usuários que nós temos nas tabelas hash
            TcpClient[] tcpClientes = new TcpClient[ChatServer.htUsuarios.Count];

            // Copia os objetos TcpClient para um array
            ChatServer.htUsuarios.Values.CopyTo(tcpClientes, 0);

            // Stream que irá enviar a mensagem para cada usuário
            StreamWriter swEnviador;

            // Laço sobre a lista de clientes TCP
            for (int i = 0; i < tcpClientes.Length; i++)
            {
                // Tentando enviar a mensagem para cada
                try
                {
                    // Se a mensagem está vazia ou a conexão é null, cai fora
                    if (mensagem.Trim() == "" || tcpClientes[i] == null)
                    {
                        continue;
                    }

                    // Envia a mensagem para o atual usuário do loop
                    swEnviador = new StreamWriter(tcpClientes[i].GetStream());

                    // Criando o pacote para enviar
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.MensagemAdministrador;
                    pacoteEnviar.Usuario = ChatServer.NomeAdministrador;
                    pacoteEnviar.Mensagem = ChatServer.NomeAdministrador + ": " + mensagem;

                    // Serializa o pacote
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);

                    swEnviador.WriteLine(objEnviar);
                    swEnviador.Flush();
                    swEnviador = null;
                }
                catch // Se ocorreu algum problema o usuário não está mais conectado. Vou remover ele
                {
                    RemoveUsuario(tcpClientes[i]);
                }
            }
        }


        // Envia a lista de usuário atualizada para todos os usuários conectados
        public static void EnviaListaDeUsuariosAtualizada()
        {
            Hashtable listaDeUsuariosAtualizadaEnviar = new Hashtable();
            foreach (DictionaryEntry entry in ChatServer.htUsuarios)
            {
                // Cria lista valor/valor
                listaDeUsuariosAtualizadaEnviar.Add(entry.Key, entry.Key);
            }

            StreamWriter swSenderSender;

            // Create an array of TCP clients, the size of the number of users we have
            TcpClient[] tcpClients = new TcpClient[ChatServer.htUsuarios.Count];

            // Copy the TcpClient objects into the array
            ChatServer.htUsuarios.Values.CopyTo(tcpClients, 0);

            // Loop through the list of TCP clients
            for (int i = 0; i < tcpClients.Length; i++)
            {
                // Try sending a message to each
                try
                {
                    // If the message is blank or the connection is null, break out
                    if (tcpClients[i] == null)
                    {
                        continue;
                    }

                    // Send the message to the current user in the loop
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());

                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.ListaDeUsuarios;
                    pacoteEnviar.Usuario = ChatServer.NomeAdministrador;
                    pacoteEnviar.ListaDeUsuarios = listaDeUsuariosAtualizadaEnviar;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);

                    swSenderSender.WriteLine(objEnviar);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch (Exception ex) // If there was a problem, the user is not there anymore, remove him
                {
                    RemoveUsuario(tcpClients[i]);
                    throw ex;
                }
            }
        }


        // Envia mensagens de um usuário para todos os outros
        public static void EnviaMensagem(Pacote pacote)
        {
            StreamWriter swSenderSender;

            // First of all, show in our application who says what
            e = new StatusChangedEventArgs(pacote.Usuario + " disse: " + pacote.Mensagem);
            OnStatusChanged(e);

            // Create an array of TCP clients, the size of the number of users we have
            TcpClient[] tcpClients = new TcpClient[ChatServer.htUsuarios.Count];
            // Copy the TcpClient objects into the array
            ChatServer.htUsuarios.Values.CopyTo(tcpClients, 0);
            // Loop through the list of TCP clients
            for (int i = 0; i < tcpClients.Length; i++)
            {
                // Try sending a message to each
                try
                {
                    // If the message is blank or the connection is null, break out
                    if (pacote.Mensagem.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    // Send the message to the current user in the loop
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());

                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.Mensagem;
                    pacoteEnviar.Mensagem = pacote.Usuario + " disse: " + pacote.Mensagem;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);

                    swSenderSender.WriteLine(objEnviar);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch // If there was a problem, the user is not there anymore, remove him
                {
                    RemoveUsuario(tcpClients[i]);
                }
            }
        }


        // Envia mensagens de um usuário para outro usuário específico apenas
        public static void EnviaMensagemPrivada(Pacote pacote)
        {
            StreamWriter swEnviar;

            // Exibe na aplicação quem disse o que e para quem
            e = new StatusChangedEventArgs(pacote.Usuario + " em reservado para " + pacote.UsuarioDestino + " disse: " + pacote.Mensagem);
            OnStatusChanged(e);

            TcpClient clienteTCPOrigem = null;
            TcpClient clienteTCPDestino = null;
            try
            {
                // Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                if ((clienteTCPDestino = (TcpClient)ChatServer.htUsuarios[pacote.UsuarioDestino]) != null)
                {
                    // Envia a mensagem para o usuário específico
                    swEnviar = new StreamWriter(clienteTCPDestino.GetStream());
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.MensagemPrivada;
                    pacoteEnviar.Mensagem = pacote.Usuario + " em reservado disse: " + pacote.Mensagem;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    swEnviar = null;
                }
                // Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                if ((clienteTCPOrigem = (TcpClient)ChatServer.htUsuarios[pacote.Usuario]) != null)
                {
                    // Envia a mensagem para o usuário específico
                    swEnviar = new StreamWriter(clienteTCPOrigem.GetStream());
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.MensagemPrivada;
                    pacoteEnviar.Mensagem = "Você em reservado para " + pacote.UsuarioDestino + " disse: " + pacote.Mensagem;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    swEnviar = null;
                }
            }
            catch // Se tiver algum problema provavelmente o usuário não está mais conectado, vou remover ele da lista
            {
                RemoveUsuario(clienteTCPDestino);
            }
        }


        #region Métodos sobre envio de arquivo
        // Quando o Usuario envia uma mensagem dizendo que deseja enviar um arquivo para o UsuarioDestino
        public static void EnviarArquivo(Pacote pacote)
        {
            StreamWriter swEnviar;

            // Exibe na aplicação quem quer enviar o arquivo e para quem
            e = new StatusChangedEventArgs(pacote.Usuario + " quer enviar o arquivo " + pacote.NomeArquivo + " arquivo para " + pacote.UsuarioDestino);
            OnStatusChanged(e);

            TcpClient clienteTCPOrigem = null;
            TcpClient clienteTCPDestino = null;
            try
            {
                // Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                if ((clienteTCPDestino = (TcpClient)ChatServer.htUsuarios[pacote.UsuarioDestino]) != null)
                {
                    // Envia a mensagem para o usuário específico
                    swEnviar = new StreamWriter(clienteTCPDestino.GetStream());
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.EnviarArquivo;
                    pacoteEnviar.Mensagem = pacote.Usuario + " deseja lhe enviar o arquivo " + pacote.NomeArquivo;
                    pacoteEnviar.Usuario = pacote.Usuario;
                    pacoteEnviar.UsuarioDestino = pacote.UsuarioDestino;
                    pacoteEnviar.NomeArquivo = pacote.NomeArquivo;
                    pacoteEnviar.TamanhoArquivo = pacote.TamanhoArquivo;
                    pacoteEnviar.ConteudoArquivo = pacote.ConteudoArquivo;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    swEnviar = null;
                }
                // Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                if ((clienteTCPOrigem = (TcpClient)ChatServer.htUsuarios[pacote.Usuario]) != null)
                {
                    // Envia a mensagem para o usuário específico
                    swEnviar = new StreamWriter(clienteTCPOrigem.GetStream());
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.MensagemPrivada;
                    pacoteEnviar.Mensagem = "Você deseja enviar o arquivo " + pacote.NomeArquivo + " para " + pacote.UsuarioDestino;
                    pacoteEnviar.Usuario = pacote.Usuario;
                    pacoteEnviar.UsuarioDestino = pacote.UsuarioDestino;
                    pacoteEnviar.NomeArquivo = pacote.NomeArquivo;
                    pacoteEnviar.TamanhoArquivo = pacote.TamanhoArquivo;
                    pacoteEnviar.ConteudoArquivo = pacote.ConteudoArquivo;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    swEnviar = null;
                }
            }
            catch // Se tiver algum problema provavelmente o usuário não está mais conectado, vou remover ele da lista
            {
                RemoveUsuario(clienteTCPDestino);
            }
        }


        // Quando o usuário aceita receber o arquivo
        public static void AceiteDoArquivo(Pacote pacote)
        {
            StreamWriter swEnviar;

            // Exibe na aplicação quem quer enviar o arquivo e para quem
            e = new StatusChangedEventArgs(pacote.Usuario + " aceitou o arquivo " + pacote.NomeArquivo + " de " + pacote.UsuarioDestino);
            OnStatusChanged(e);

            TcpClient clienteTCPOrigem = null;
            TcpClient clienteTCPDestino = null;
            try
            {
                // Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                if ((clienteTCPDestino = (TcpClient)ChatServer.htUsuarios[pacote.UsuarioDestino]) != null)
                {
                    // Envia a mensagem para o usuário específico
                    swEnviar = new StreamWriter(clienteTCPDestino.GetStream());
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.AceiteDoArquivo;
                    pacoteEnviar.Usuario = pacote.Usuario;
                    pacoteEnviar.UsuarioDestino = pacote.UsuarioDestino;
                    pacoteEnviar.Mensagem = pacote.Mensagem;
                    pacoteEnviar.NomeArquivo = pacote.NomeArquivo;
                    pacoteEnviar.TamanhoArquivo = pacote.TamanhoArquivo;
                    pacoteEnviar.ConteudoArquivo = pacote.ConteudoArquivo;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    swEnviar = null;
                }
                // Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                if ((clienteTCPOrigem = (TcpClient)ChatServer.htUsuarios[pacote.Usuario]) != null)
                {
                    // Envia a mensagem para o usuário específico
                    swEnviar = new StreamWriter(clienteTCPOrigem.GetStream());
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.MensagemPrivada;
                    pacoteEnviar.Mensagem = "Você aceitou o arquivo: " + pacote.NomeArquivo + " de " + pacote.UsuarioDestino;
                    pacoteEnviar.NomeArquivo = pacote.NomeArquivo;
                    pacoteEnviar.TamanhoArquivo = pacote.TamanhoArquivo;
                    pacoteEnviar.ConteudoArquivo = null;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    swEnviar = null;
                }
            }
            catch // Se tiver algum problema provavelmente o usuário não está mais conectado, vou remover ele da lista
            {
                RemoveUsuario(clienteTCPDestino);
            }
        }


        // Quando o usuário se recusa a receber o arquivo
        public static void ArquivoRecusado(Pacote pacote)
        {
            StreamWriter swEnviar;

            // Exibe na aplicação quem quer enviar o arquivo e para quem
            e = new StatusChangedEventArgs(pacote.Usuario + " recusou o arquivo " + pacote.NomeArquivo + " de " + pacote.UsuarioDestino);
            OnStatusChanged(e);

            TcpClient clienteTCPOrigem = null;
            TcpClient clienteTCPDestino = null;
            try
            {
                // Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                if ((clienteTCPDestino = (TcpClient)ChatServer.htUsuarios[pacote.UsuarioDestino]) != null)
                {
                    // Envia a mensagem para o usuário específico
                    swEnviar = new StreamWriter(clienteTCPDestino.GetStream());
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.ArquivoRecusado;
                    pacoteEnviar.Mensagem = pacote.Usuario + " recusou o arquivo " + pacote.NomeArquivo;
                    pacoteEnviar.NomeArquivo = pacote.NomeArquivo;
                    pacoteEnviar.TamanhoArquivo = pacote.TamanhoArquivo;
                    pacoteEnviar.ConteudoArquivo = null;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    swEnviar = null;
                }
                // Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                if ((clienteTCPOrigem = (TcpClient)ChatServer.htUsuarios[pacote.Usuario]) != null)
                {
                    // Envia a mensagem para o usuário específico
                    swEnviar = new StreamWriter(clienteTCPOrigem.GetStream());
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.MensagemPrivada;
                    pacoteEnviar.Mensagem = "Você recusou o arquivo " + pacote.NomeArquivo + " de " + pacote.UsuarioDestino;
                    pacoteEnviar.NomeArquivo = pacote.NomeArquivo;
                    pacoteEnviar.TamanhoArquivo = pacote.TamanhoArquivo;
                    pacoteEnviar.ConteudoArquivo = null;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    swEnviar = null;
                }
            }
            catch // Se tiver algum problema provavelmente o usuário não está mais conectado, vou remover ele da lista
            {
                RemoveUsuario(clienteTCPDestino);
            }
        }


        // Depois de aceito, quando o arquivo realmente é enviado no campo Mensagem
        public static void EnviandoArquivo(Pacote pacote)
        {
            StreamWriter swEnviar;

            // Exibe na aplicação quem quer enviar o arquivo e para quem
            e = new StatusChangedEventArgs(pacote.Usuario + " enviando o arquivo " + pacote.NomeArquivo + " para " + pacote.UsuarioDestino);
            OnStatusChanged(e);

            //TcpClient clienteTCPOrigem = null;
            TcpClient clienteTCPDestino = null;
            try
            {
                // Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                if ((clienteTCPDestino = (TcpClient)ChatServer.htUsuarios[pacote.UsuarioDestino]) != null)
                {
                    // Envia a mensagem para o usuário específico
                    swEnviar = new StreamWriter(clienteTCPDestino.GetStream());
                    Pacote pacoteEnviar = new Pacote();
                    pacoteEnviar.Comando = Comando.EnviandoArquivo;
                    pacoteEnviar.Usuario = pacote.Usuario;
                    pacoteEnviar.UsuarioDestino = pacote.UsuarioDestino;
                    pacoteEnviar.Mensagem = pacote.Mensagem;
                    pacoteEnviar.NomeArquivo = pacote.NomeArquivo;
                    pacoteEnviar.TamanhoArquivo = pacote.TamanhoArquivo;
                    pacoteEnviar.ConteudoArquivo = pacote.ConteudoArquivo;
                    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                    swEnviar.WriteLine(objEnviar);
                    swEnviar.Flush();
                    swEnviar = null;
                }
                //// Busca o cliente pela própria chave da hash table, não precisa de laço for e tal...
                //if ((clienteTCPOrigem = (TcpClient)ChatServer.htUsuarios[pacote.Usuario]) != null)
                //{
                //    // Envia a mensagem para o usuário específico
                //    swEnviar = new StreamWriter(clienteTCPOrigem.GetStream());
                //    Pacote pacoteEnviar = new Pacote();
                //    pacoteEnviar.Comando = Comando.MensagemPrivada;
                //    pacoteEnviar.Mensagem = "Você em quer enviar o arquivo " + pacote.Mensagem + " para " + pacote.UsuarioDestino;
                //    string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
                //    swEnviar.WriteLine(objEnviar);
                //    swEnviar.Flush();
                //    swEnviar = null;
                //}
            }
            catch // Se tiver algum problema provavelmente o usuário não está mais conectado, vou remover ele da lista
            {
                RemoveUsuario(clienteTCPDestino);
            }
        }
        #endregion


        public void IniciaAEscuta()
        {
            // Pega o IP do primeiro dispositivo de rede, isto pode causar problemas por exemplo se o computador tiver 2 redes
            //IPAddress ipaLocal = EnderecoIP;

            // Cria o TCP listener usando o IP do servidor e a porta especificada
            ChatServer.tlsCliente = new TcpListener(this.EnderecoIP, this.porta);

            // Inicia o TCP listener e escuta por conexões
            ChatServer.tlsCliente.Start();

            // The while loop will check for true in this before checking for connections
            this.ServidorRodando = true;

            // Start the new tread that hosts the listener
            this.thrListener = new Thread(ContinuaEscutando);

            this.thrListener.Start();
        }


        public void ParaAEscuta()
        {
            this.ServidorRodando = false;

            TcpClient tcpc = new TcpClient();
            tcpc.Connect(this.EnderecoIP, this.porta);

            StreamWriter swSenderSender = new StreamWriter(tcpc.GetStream());
            Pacote pacoteEnviar = new Pacote();
            pacoteEnviar.Comando = Comando.PararEscuta;
            pacoteEnviar.Usuario = ChatServer.NomeAdministrador;
            pacoteEnviar.Mensagem = ChatServer.IDAdministrador;
            string objEnviar = JsonConvert.SerializeObject(pacoteEnviar);
            swSenderSender.WriteLine(objEnviar);
            swSenderSender.Flush();
            swSenderSender = null;
            tcpc.Close();

            //ChatServer.tlsClient.Stop();
            //this.thrListener.Abort();
        }


        private void ContinuaEscutando()
        {
            // Enquanto o servidor estiver rodando...
            while (this.ServidorRodando == true)
            {
                try
                {
                    // Aceita a conexão pendente
                    this.tcpCliente = ChatServer.tlsCliente.AcceptTcpClient();

                    Conexao novaConexao = null;

                    #region Analisa Pacote
                    // Antes de criar a conexão, vou analisar o pacote, pois se for um pacote para parar de escutar, aqui é o único ponto onde consigo para o servidor TCP
                    StreamReader srReceiver = new StreamReader(this.tcpCliente.GetStream());

                    // Lê as informações de conta que vieram do cliente
                    string conteudoRecebido = srReceiver.ReadLine();

                    if (conteudoRecebido != null)
                    {
                        Pacote pacoteRecebido = JsonConvert.DeserializeObject<Pacote>(conteudoRecebido);
                        // Se é um pacote para parar a escuta, então paro!
                        // ATENÇÃO: Tomar cuidado pois aqui qualquer usuário engraçadinho que enviar um pacote com este comando irá parar o servidor.
                        // Adicionar uma validação para verificar se o comando realmente é do Administrador.
                        if (pacoteRecebido.Comando == Comando.PararEscuta && pacoteRecebido.Usuario == ChatServer.NomeAdministrador && pacoteRecebido.Mensagem == ChatServer.IDAdministrador)
                        {
                            ChatServer.tlsCliente.Stop();
                            this.tcpCliente.Close();
                            srReceiver.Close();
                            return;
                        }

                        if (pacoteRecebido.Comando == Comando.Conectar)
                        {
                            novaConexao = new Conexao(this.tcpCliente, pacoteRecebido);
                            //return;
                        }
                    }
                    #endregion

                    if (novaConexao == null)
                    {
                        // Cria uma nova instância da conexão
                        novaConexao = new Conexao(tcpCliente);
                    }
                }
                catch (SocketException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
