using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;

namespace Core.Negocio
{
    public class StVerificaServicoExistente : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {

            TipoServico tipoServico = (TipoServico)entidade;

            tipoServico.StrBusca = tipoServico.NomeServico;

            Resultado resultado = new Fachada().Consultar(tipoServico);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                return null;
            }
            else
            {
                return "Nome de serviço já cadastrado";
            }
            
        }
    }
}
