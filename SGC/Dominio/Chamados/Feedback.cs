using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Feedback : EntidadeDominio
    {
        public int NotaAtendimento { get; set; }
        public int NotaAtendente { get; set; }
        public string Comentarios { get; set; }
        public string Status { get; set; }

        public Ticket Ticket { get; set; }



        public Feedback()
        {
            NotaAtendente = 0;
            NotaAtendimento = 0;
            Comentarios = string.Empty;
            Status = string.Empty;
            Ticket = new Ticket();
        }
    }
}
