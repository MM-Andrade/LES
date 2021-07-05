using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;
using Dominio.Usuarios;

namespace Core.Negocio
{
    public class StValidaUsuarioAtivo : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Usuario usuario = (Usuario)entidade;

            Resultado resultado = new Fachada().Consultar(usuario);

            if (!string.IsNullOrEmpty(resultado.Mensagem)) //usuário encontrado, retornando objeto...
            {
                return null;
            }
            else //usuário não encontrado
            {
                return "Usuário desativado, procure o administrador do sistema";
            }
        }
    }
}
