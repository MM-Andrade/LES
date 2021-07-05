using Core.Core;
using Dominio;

namespace Core.Negocio.stFeedback
{
    public class stAtualizaFeedback : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if (entidade != null)
            {
                Feedback feedback = (Feedback)entidade;

                feedback.Status = "Concluído";

                return null;

            }
            else
                return "Entidade nula";
        }
    }
}
