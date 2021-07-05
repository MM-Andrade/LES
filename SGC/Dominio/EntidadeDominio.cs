using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class EntidadeDominio : IEntidade
    {
        public int Id { get; set; }
        public DateTime DataCadastro { get; set; }
        public string StrBusca { get; set; }
        public string StrAcao { get; set; }
    }


}
