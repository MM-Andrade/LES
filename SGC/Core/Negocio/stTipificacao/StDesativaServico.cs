using Core.Core;
using Dominio;

namespace Core.Negocio
{
    class StDesativaServico : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            TipoServico tipoServico = (TipoServico)entidade;
            if (entidade != null)
            {
                if (tipoServico.StrAcao == "Desativar")
                {
                    tipoServico.Status = "Inativo";
                }
            }

            return null;
        }
    }
}
