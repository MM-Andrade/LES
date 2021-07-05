using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;

namespace Core.Negocio.stCalculosSLA
{
    public class StVerificaSlaViolado : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if (entidade != null)
            {
                SlaTicket slaTicket = (SlaTicket)entidade;

                slaTicket.StrAcao = "JobVerificaSlaViolado";

                Resultado resultado = new Fachada().Atualizar(slaTicket);
                if (string.IsNullOrEmpty(resultado.Mensagem))
                {//deu tudo certo e atualizou...
                    return null;
                }
                else
                {
                    return "Erro na regra de calculo de SLA";
                }

            }
            else
            {
                return "Entidade Nula";
            }
        }
    }
}

