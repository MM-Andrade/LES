using Core.Core;
using Dominio;
using System;

namespace Core.Negocio.Chamados
{
    public class stGeraNumeroChamado : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Ticket ticket = (Ticket)entidade;

            if (entidade != null)
            {
                ticket.CodigoTicket = GeraIdTicket();
                return null;
            }
            else
            {
                return "Entidade Nula";
            }

        }

        public static string GeraIdTicket()
        {
            string Codigo;

            //gera o numero do incidente com Dia Mes Ano Hora Minuto Milisegundo
            //131218163322
            Codigo = DateTime.Now.ToString("ddMMyyHHmmff");
            
            //var chars = "000123456789";
            //var random = new Random();
            //var result = new string(
            //    Enumerable.Repeat(chars, 5)
            //              .Select(s => s[random.Next(s.Length)])
            //              .ToArray());

            return "INC" + Codigo;
        }
    }
}
