using Core.Core;
using Dominio;
using Dominio.Chamados;

namespace Core.Negocio.stServicos
{
    public class stAtribuiTicketServico : IStrategy
    {
        /// <summary>
        /// regra que atribui o ID do ticket para o item de serviço utilizado 
        /// </summary>
        /// <param name="entidade"></param>
        /// <returns></returns>
        public string Processar(EntidadeDominio entidade)
        {
            if (entidade != null)
            {
                ServicosUtilizados servicosUtilizados = new ServicosUtilizados();

                //servicosUtilizados.GetTicket.Id = ticket.Id;

                return null;

            }
            else
            {
                return "Entidade Nula";
            }
        }
    }
}
