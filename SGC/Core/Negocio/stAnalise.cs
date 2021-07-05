using Core.Core;
using Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Negocio
{
    public class stAnalise : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if(entidade != null)
            {
                AnaliseTicket analiseTicket = new AnaliseTicket();

                if (analiseTicket.DataInicioAnalise.Year > DateTime.Now.Year && analiseTicket.DataFinalAnalise.Year > DateTime.Now.Year)
                {
                    return "Desculpe, a análise só funciona no ano ano corrente... Dica: usa algo dentro do ano de " + Convert.ToString(DateTime.Now.Year) ;
                }
                else if(analiseTicket.DataInicioAnalise.Year > DateTime.Now.Year)
                {
                    return "Desculpe, a data de inicio tem que ser dentro do ano corrente...";
                }
                else if(analiseTicket.DataFinalAnalise.Year > DateTime.Now.Year)
                {
                    return "Desculpe, a data de final tem que ser dentro do ano corrente...";
                }

                return null;
            }


            return "entidade nula";
        }
    }
}
