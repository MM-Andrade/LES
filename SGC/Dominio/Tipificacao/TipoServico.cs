using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class TipoServico : EntidadeDominio
    {
        public string NomeServico { get; set; }
        public Departamento Departamento { get; set; }
        public Prioridade Prioridade { get; set; }
        public string Status { get; set; }

        public TipoServico()
        {
            NomeServico = string.Empty;
            Departamento = new Departamento();
            Prioridade = new Prioridade();
            Status = string.Empty;
        }
    }
}
