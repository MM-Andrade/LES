using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class SlaTicket : EntidadeDominio
    {
        public string Status { get; set; }
        public string PercentilAtendimento { get; set; }
        public DateTime DataViolacao { get; set; }
        public DateTime DataPrevista { get; set; }
        public Ticket Ticket { get; set; }                  //apenas para navegação
        public string Motivo { get; set; }

        public SlaTicket()
        {
            Status = string.Empty;
            PercentilAtendimento = string.Empty;
            Motivo = string.Empty;
            DataViolacao = new DateTime();
            DataPrevista = new DateTime();

        }
    }
}
