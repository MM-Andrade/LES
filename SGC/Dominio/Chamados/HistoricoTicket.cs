using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class HistoricoTicket : EntidadeDominio
    {
        public DateTime DataAtualizacao { get; set; }
        public string Status { get; set; }
        public string StatusAnterior { get; set; }
        public string FilaDeAtendimento { get; set; }
        public string TecnicoResponsavel { get; set; }
        public string  Prioridade { get; set; }
        public string Descricao { get; set; }
        public ComentariosTicket ComentariosTicket { get; set; }
        public string CodigoTicket { get; set; }


        public HistoricoTicket()
        {
            DataAtualizacao = new DateTime();
            Status = string.Empty;
            StatusAnterior = string.Empty;
            FilaDeAtendimento = string.Empty;
            TecnicoResponsavel = string.Empty;
            Prioridade = string.Empty;
            Descricao = string.Empty;
            ComentariosTicket = new ComentariosTicket();
            CodigoTicket = string.Empty;

        }

    }
}
