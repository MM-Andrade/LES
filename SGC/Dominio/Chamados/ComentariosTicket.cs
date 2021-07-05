using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class ComentariosTicket : EntidadeDominio
    {
        public string Comentario { get; set; }
        public Usuario Usuario { get; set; }

        //para buscar o id do ticket (apenas pra isso)
        public string CodigoTicket { get; set; }

        public ComentariosTicket()
        {
            Comentario = string.Empty;
            CodigoTicket = string.Empty;
            Usuario = new Usuario();
            DataCadastro = new DateTime();

        }
    }
}
