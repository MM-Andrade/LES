using Core.Aplicacao;
using Core.Core;
using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Negocio.stEstoque
{
    public class stAlteracaoEstoqueChamado : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if (entidade != null)
            {

                AlteracaoEstoque alteracaoEstoque = (AlteracaoEstoque)entidade;
                if (alteracaoEstoque.Estoque.StrAcao == "EditarProduto")
                {
                    return null;
                }
                else
                {
                    alteracaoEstoque.QuantidadeBaixa = 1;
                    alteracaoEstoque.Acao = "Remoção";
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

