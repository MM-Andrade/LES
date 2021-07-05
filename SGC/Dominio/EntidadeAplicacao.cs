using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class EntidadeAplicacao : IEntidade
    {
        public DateTime DataInicioAnalise { get; set; }
        public DateTime DataFinalAnalise { get; set; }
        public string StrAcao { get; set; }
    }
}
