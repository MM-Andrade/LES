using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class AlteracaoEstoque : EntidadeDominio
    {
        public string Motivo { get; set; }
        public int QuantidadeBaixa { get; set; }
        public string Acao { get; set; }
        public Estoque Estoque { get; set; }
        public Usuario Usuario { get; set; }

        public AlteracaoEstoque()
        {
            Motivo = string.Empty;
            QuantidadeBaixa = 0;
            Acao = string.Empty;
            Estoque = new Estoque();
            Usuario = new Usuario();
        }
    }
}
