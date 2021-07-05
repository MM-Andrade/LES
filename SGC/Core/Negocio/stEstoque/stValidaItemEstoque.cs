using Core.Core;
using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Negocio.stEstoque
{
    public class stValidaItemEstoque : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Estoque estoque = (Estoque)entidade;

            if (entidade != null)
            {
                if (string.IsNullOrEmpty(estoque.Item))
                    return string.Format("Campo {0} vazio ou preenchido de forma errada!\n", "Item");
                else if (estoque.QuantidadeEstoque == 0)
                    return string.Format("Campo {0} vazio ou preenchido de forma errada!\n", "Quantidade");
                else if (string.IsNullOrEmpty(estoque.Unidade))
                    return string.Format("Campo {0} vazio ou preenchido de forma errada!\n", "Unidade");
                else if (string.IsNullOrEmpty(estoque.codigoean) && estoque.codigoean.Length > 13)
                    return string.Format("Campo {0} vazio ou preenchido de forma errada!\n", "EAN");
                else if (estoque.Departamento.Id == 0)
                    return string.Format("Campo {0} vazio ou preenchido de forma errada!\n", "Departamento");
                else
                return null;
            }
            else
                return "Entidade nula";

        }
    }
}
