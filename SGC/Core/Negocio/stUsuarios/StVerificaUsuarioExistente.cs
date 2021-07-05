using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;
using Dominio.Usuarios;
using System.Linq;

namespace Core
{
    public class StVerificaUsuarioExistente : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Usuario usuario = (Usuario)entidade;
            var obj = (Usuario)entidade;

            usuario.StrBusca = usuario.Email;

            Resultado resultado = new Fachada().Consultar(usuario);

            obj = (Usuario)resultado.Entidades.FirstOrDefault();

            if (usuario.Email == obj.Email)
            {
                return "Usuário já cadastrado";
            }
            else
            {
                return null;
            }
        }
    }
}
