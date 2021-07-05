using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Estoque : EntidadeDominio
    {
        public string Item { get; set; }
        public int QuantidadeEstoque { get; set; }
        public string Unidade { get; set; }
        public string codigoean { get; set; }


        public Departamento Departamento { get; set; }
        public Ticket GetTicket { get; set; }
        public Usuario GetUsuario { get; set; }

        public Estoque()
        {
            Item = string.Empty;
            QuantidadeEstoque = 0;
            Unidade = string.Empty;
            Departamento = new Departamento();
            GetTicket = new Ticket();
            GetUsuario = new Usuario();
        }



    }
}
