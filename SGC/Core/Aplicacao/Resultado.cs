using Dominio;
using System.Collections.Generic;

namespace Core.Aplicacao
{
    public class Resultado
    {
        public string Mensagem { get; set; }
        public List<IEntidade> Entidades { get; set; }

        public Resultado()
        {
            Entidades = new List<IEntidade>();
            Mensagem = string.Empty;
        }
    }
}
