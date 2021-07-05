using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;
using Dominio.Usuarios;
using System.Linq;

namespace Core.Negocio
{
    public class StAlterarSenha : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Usuario usuario = (Usuario)entidade;
            var obj = (Usuario)entidade;

            usuario.StrAcao = "Login";

            if (usuario.Senha != usuario.ConfirmarSenha)
            {
                return "As senhas não conferem...";
            }

            Resultado resultado = new Fachada().Consultar(usuario);

            if (string.IsNullOrEmpty(resultado.Mensagem))
            {
                obj = (Usuario)resultado.Entidades.FirstOrDefault();
                if(obj.Senha != usuario.Senha)
                {
                    return "A senha atual está incorreta!";
                }
                else
                {
                    usuario.StrAcao = "AlterarSenha";
                    return null;
                }
            }
            else
            {
                return "Usuário não encontrado";
            }
        }
    }
}
