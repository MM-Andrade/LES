using Core.Core;
using Dominio;
using System;

namespace Core.Negocio.Chamados
{
    public class stNovoChamado : IStrategy
    {
        //regra para definir um novo chamado
        public string Processar(EntidadeDominio entidade)
        {
            Ticket ticket = (Ticket)entidade;
            if (entidade != null)
            {
                ticket.Status = "Novo";
                ticket.DataCadastro = DateTime.Now;
                ticket.DataAtualizacao = ticket.DataCadastro; //apenas para iniciar a data de atualização...
                return null;
            }
            else
            {
                return "Entidade Nula";
            }
        }
    }
}
