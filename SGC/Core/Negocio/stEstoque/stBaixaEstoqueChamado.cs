using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;
using System.Linq;

namespace Core.Negocio.stEstoque
{
    public class stBaixaEstoqueChamado : IStrategy
    {
        /// <summary>
        /// regra responsável por dar baixa do estoque
        /// </summary>
        /// <param name="entidade"></param>
        /// <returns></returns>
        public string Processar(EntidadeDominio entidade)
        {
            if (entidade != null)
            {

                Estoque estoque = (Estoque)entidade;

                if (estoque.GetTicket.StrAcao == "ResolverChamado")
                {
                    Resultado resultado = new Resultado();

                    resultado = new Fachada().Consultar(estoque);
                    //atribui o valor de estoque para uma variavel temporaria ticket...
                    var varEstoque = (Estoque)resultado.Entidades.FirstOrDefault();

                    //remove um item do estoque
                    estoque.QuantidadeEstoque = varEstoque.QuantidadeEstoque - 1;

                    return null;
                }
                else
                {//não, é qualquer atualizacao de estoque ...
                    return null;
                }

            }
            else
            {
                return "Entidade Nula";
            }
        }
    }
}
