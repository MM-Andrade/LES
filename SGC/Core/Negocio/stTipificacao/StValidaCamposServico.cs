using Core.Core;
using Dominio;

namespace Core.Negocio
{
    public class StValidaCamposServico : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            TipoServico tipoServico = (TipoServico)entidade;

            if (entidade != null)
            {
                if (string.IsNullOrEmpty(tipoServico.NomeServico))
                    return string.Format("Preencha um nome para o tipo do serviço! \n");
                else if (tipoServico.Prioridade.Id == 0)
                    return string.Format("Selecione uma prioridade para este atendimento! \n");
                else if (tipoServico.Departamento.Id == 0)
                    return string.Format("Selecione uma categoria para este atendimento! \n");
                else if (string.IsNullOrEmpty(tipoServico.Status))
                    return string.Format("Selecione o status da categoria!  \n");

                return null;
            }
            else
                return "Entidade nula";
        }
    }
}
