using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ChatLib
{
    public enum Comando
    {
        Conectar = 0,
        PararEscuta = 1,
        NomeReservado = 2,
        ConectadoComSucesso = 3,
        ListaDeUsuarios = 4,
        Mensagem = 5,
        MensagemPrivada = 6,
        MensagemAdministrador = 7,
        EnviarArquivo = 8,
        AceiteDoArquivo = 9,
        ArquivoRecusado = 10,
        EnviandoArquivo = 11
    }

    public class Pacote
    {
        public Comando Comando;
        public string Usuario;
        public string UsuarioDestino;
        public string Mensagem;
        public string NomeArquivo;
        public string TamanhoArquivo;
        public string ConteudoArquivo;
        public Hashtable ListaDeUsuarios;
    }
}
