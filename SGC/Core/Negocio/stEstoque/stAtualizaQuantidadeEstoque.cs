using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Negocio.stEstoque
{
    public class stAtualizaQuantidadeEstoque : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if (entidade != null)
            {
                Estoque estoque = (Estoque)entidade;
                if (estoque.StrAcao == "EditarProduto")
                {
                    estoque.StrBusca = estoque.codigoean;
                    Resultado resultado = new Fachada().Consultar(estoque);

                    if (string.IsNullOrEmpty(resultado.Mensagem))
                    {   //atribui o valor de resultado em uma variavel para trabalharmos

                        //var item = resultado.Entidades.FirstOrDefault() as Estoque;
                        
                        //if(item.QuantidadeEstoque < estoque.QuantidadeEstoque) // o valor passado no estoque é menor do que ja existe?
                        //{//sim

                        //    estoque.SetAlteracaoEstoque.Motivo = "Remoção de produto";
                        //}

                        //else if(item.QuantidadeEstoque > estoque.QuantidadeEstoque)
                        //{
                        //    estoque.SetAlteracaoEstoque.Motivo = "Adição de produto, foram removidos " + estoque.QuantidadeEstoque + "do produto de código " + estoque.codigoean + "-" + estoque.Item;
                        //}

                        return null;
                    }
                    else
                    {
                        return "Não foi possível atualizar a quantidade cadastrada";
                    }
                }
                else
                    //não é essa regra para executar.
                    return null;
            }
            else
                return "Entidade Nula";
        }
    }
}
