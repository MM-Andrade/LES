using Core.Core;
using Dominio;
using Dominio.Tipificacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Negocio.stTipificacao
{
    public class stAdicionaItemServico : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if(entidade != null)
            {
                ItemServico itemServico = (ItemServico)entidade;

                if (itemServico.Dpto.Id <= 0)
                    return "Departamento Obrigatório";
                else if (string.IsNullOrEmpty(itemServico.Item))
                    return "Nome do item obrigatório";

                else
                    return null;
            }
            else
                return "Entidade nula";

            
        }
    }
}
