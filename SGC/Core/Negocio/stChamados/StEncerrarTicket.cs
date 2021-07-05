using Core.Core;
using Dominio;
using System;

namespace Core.Negocio.stChamados
{
    public class StEncerrarTicket : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {

            if (entidade != null)
            {

                Ticket ticket = (Ticket)entidade;


                if (ticket.StrAcao != "JobEncerrarTicket")
                {
                    //para não dar erro no job, mantenha null
                    return null;

                }
                
                    //atualiza a data de encerramento do ticket
                    ticket.DataEncerramento = DateTime.Now;
                    //atribui o status para fechado
                    ticket.Status = "Fechado Resolvido";
                    
                

                //insere um comentário..
                    //ticket.ComentariosTicket.Comentario = "Ticket encerrado automaticamente pelo sistema após 3 dias no estado de resolvido";

                    return null; //para sair da regra ok :)
            }
            else
            {
                return "Entidade Nula";
            }
               
        }
    }
}
