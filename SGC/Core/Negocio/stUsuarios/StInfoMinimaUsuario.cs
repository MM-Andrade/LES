using Core.Core;
using Dominio;
using Dominio.Usuarios;
using System;

namespace Core.Negocio
{
    public class StInfoMinimaUsuario : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Usuario usuario = (Usuario)entidade;

            if (entidade != null)
            {
                if (string.IsNullOrEmpty(usuario.Nome))
                    return string.Format("Campo {0} vazio ou preenchido de forma errada!\n", "Nome");
                else if (string.IsNullOrEmpty(usuario.Sobrenome))
                    return string.Format("Campo {0} vazio ou preenchido de forma errada!\n", "Sobrenome");
                else if (string.IsNullOrEmpty(usuario.Genero))
                    return string.Format("Campo {0} vazio ou preenchido de forma errada!\n", "Genero");
                else if (string.IsNullOrEmpty(Convert.ToString(usuario.DataNascimento)))
                    return string.Format("Campo {0} vazio ou preenchido de forma errada!\n", "DataNascimento");
                else if (usuario.Funcao.Id == 0 || usuario.Funcao.Id.Equals(""))
                    return string.Format("Todo usuário tem uma função! \n");
                else
                return null;
            }
            else
                return "Entidade nula";
           
        }
    }
}
