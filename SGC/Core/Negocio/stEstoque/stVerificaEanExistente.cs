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
    public class stVerificaEanExistente : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Estoque estoque = (Estoque)entidade;

            estoque.StrBusca = estoque.codigoean;

            Resultado resultado = new Fachada().Consultar(estoque);
            
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                return null;
            }
            else
            {
                //esse método pra funcionar tem que validar muita coisa... fica pra próxima.
                //var item = resultado.Entidades.FirstOrDefault() as Estoque;
                ////adiciona a quantidade do item do estoque cadastrado
                //item.QuantidadeEstoque = item.QuantidadeEstoque + estoque.QuantidadeEstoque;
                //resultado = new Fachada().Atualizar(estoque);

                return "Código EAN já cadastrado";
            }
        }
    }
}
