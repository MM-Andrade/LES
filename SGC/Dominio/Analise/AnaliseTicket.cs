using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class AnaliseTicket : EntidadeAplicacao
    {
        public Ticket GetTicket { get; set; }
        public double ContadorTipo { get; set; }
        public string Data { get; set; }


        public AnaliseTicket()
        {
            GetTicket = new Ticket();
            ContadorTipo = 0.0;
            Data = string.Empty;
        }
    }
}
