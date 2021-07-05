using Dominio.Tipificacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Chamados
{
    public class ServicosUtilizados : EntidadeDominio
    {
        public Ticket GetTicket { get; set; }
        public ItemServico GetItemServico { get; set; }

        public ServicosUtilizados()
        {
            GetTicket = new Ticket();
            GetItemServico = new ItemServico();
        }
    }
}
