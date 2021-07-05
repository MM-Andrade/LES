using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Prioridade : EntidadeDominio
    {
        public int NivelPrioridade { get; set; }
        public string DescricaoPrioridade { get; set; }
        public int TempoAtendimento { get; set; }
    }

}
