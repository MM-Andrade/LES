using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Usuarios
{
    public class Usuario : EntidadeDominio
    {
       
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Genero { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string NovaSenha { get; set; }
        public string ConfirmarSenha { get; set; }
        public bool UsuarioAtivo { get; set; }

        public Funcao Funcao { get; set; }
        public GrupoAtendimento GrupoAtendimento { get; set; }
        public Endereco Endereco { get; set; }



        public Usuario()
        {
            Nome = string.Empty;
            Sobrenome = string.Empty;
            Genero = string.Empty;
            UsuarioAtivo = new bool();
            Email = string.Empty;
            Senha = string.Empty;
            Funcao = new Funcao();
            GrupoAtendimento = new GrupoAtendimento();
            Endereco = new Endereco();
        }
    }
}
