using Core.Core;
using Dominio;

namespace Core.Negocio.stChamados
{
    public class stResolverTicket : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if (entidade != null)
            {
                Ticket ticket = (Ticket)entidade;

                if (ticket.StrAcao == "ResolverChamado")
                {
                    if (ticket.ItensServico.Count <= 0)
                    {
                        return "É obrigatório um item de serviço utilizado";
                    }
                }
                return null; //para sair da regra ok :)
            }
            else
            {
                return "Entidade Nula";
            }

        }
    }
}

