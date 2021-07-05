using Core.Core;
using Dominio;
using Dominio.Usuarios;

namespace Core.Negocio
{
    public class StValidarEmail : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Usuario usuario = (Usuario)entidade;

            if (string.IsNullOrEmpty(usuario.Email))
                return string.Format("Preencher EMAIL \n");
            else if (entidade != null)
            {
                if (!ValidarEmail(usuario.Email))
                    return string.Format("Email inválido \n", "EMAIL");
            }
            return null;
        }

        public bool ValidarEmail(string email)
        {
            if (email.Length == 0)
                return false;

            if (email.IndexOf(" ") > 0)
                return false;

            bool flgEmailValido = false;

            int indexArr = email.IndexOf('@');

            if (indexArr > 0)
            {
                int indexDot = email.IndexOf('.', indexArr);
                if (indexDot - 1 > indexArr)
                {
                    if (indexDot + 1 < email.Length)
                    {
                        string indexDot2 = email.Substring(indexDot + 1, 1);
                        if (indexDot2 != ".")
                            flgEmailValido = true;
                    }
                }
            }
            return flgEmailValido;
        }
    }
}
