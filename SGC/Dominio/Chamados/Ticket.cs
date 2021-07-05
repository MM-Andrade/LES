using Dominio.Tipificacao;
using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Ticket : EntidadeDominio
    {
        public string CodigoTicket { get; set; }
        public string TecnicoResponsavel { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string Status { get; set; }
        public DateTime DataResolucao { get; set; }
        public DateTime DataEncerramento { get; set; }
        public string TempoTotalAtendimento { get; set; }
        public string Descricao { get; set; }

        public List<int> ItensEstoque { get; set; }
        public List<int> ItensServico { get; set; }


        public Usuario Usuario { get; set; }
        public TipoServico TipoServico { get; set; }
        public HistoricoTicket HistoricoTicket { get; set; }
        public SlaTicket SlaTicket { get; set; }
        public ComentariosTicket ComentariosTicket { get; set; }
        public string InfoFeedback { get; set; }


        public Ticket()
        {
            CodigoTicket = string.Empty;
            TecnicoResponsavel = string.Empty;
            DataAtualizacao = new DateTime();
            Status = string.Empty;
            DataResolucao = new DateTime();
            DataEncerramento = new DateTime();
            TempoTotalAtendimento = string.Empty;
            Descricao = string.Empty;
            InfoFeedback = string.Empty;

            Usuario = new Usuario();
            TipoServico = new TipoServico();
            ComentariosTicket = new ComentariosTicket();
            HistoricoTicket = new HistoricoTicket();
            SlaTicket = new SlaTicket();

            ItensEstoque = new List<int>();
            ItensServico = new List<int>();
        }
    }
}
