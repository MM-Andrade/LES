using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Usuarios
{
    public class Endereco : EntidadeDominio
    {
        public string Rua { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
        public string Complemento { get; set; }
        public string Numero { get; set; }



        public Endereco()
        {
            Rua = string.Empty;
            Cidade = string.Empty;
            Bairro = string.Empty;
            Estado = string.Empty;
            Cep = string.Empty;
            Complemento = string.Empty;
            Numero = string.Empty;

        }
    }
}
