using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;
using System;
using System.Linq;

namespace Core.Negocio.Chamados
{
    public class stCalculaSLA : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if(entidade !=null)
            {
                SlaTicket slaTicket = (SlaTicket)entidade;
                Ticket ticket = slaTicket.Ticket;

                ticket.StrAcao = "ConsultarSLA";
                
                Resultado resultado = new Fachada().Consultar(ticket);
                if(string.IsNullOrEmpty(resultado.Mensagem))
                {
                    var slaT = resultado.Entidades.FirstOrDefault() as Ticket;

                    //atribui o início da data de cadastro de SLA igual ao do ticket
                    ticket.SlaTicket.DataCadastro = ticket.DataCadastro;
                    //atribui o valor dda data prevista do sla com a data calulada da consulta...
                    ticket.SlaTicket.DataPrevista = slaT.SlaTicket.DataPrevista;
                    //inicializando data de violação...
                    ticket.SlaTicket.DataViolacao = new DateTime();
                    //inicia o SLA com status OK...
                    ticket.SlaTicket.Status = "Ok";
                    return null;
                }
                else
                {
                    return "Erro na regra de calculo de SLA";
                }

            }
            else
            {
                return "Entidade Nula";
            }
        }
    }
}
