using Core.Core;
using Dominio;
using Dominio.Usuarios;
using System;
using System.Linq;

namespace Core
{
    public class StValidarSenha : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Usuario usuario = (Usuario)entidade;

            if (usuario.Senha != usuario.ConfirmarSenha)
                return "As senhas não conferem";

            if (string.IsNullOrEmpty(usuario.Senha))
                return string.Format("Senha é um campo obrigatório ");
            else if (entidade != null)
            {
                if (!VerificaSenhaForte(usuario.Senha))
                    return string.Format("\n A senha deve conter de 6 a 12 caracteres, com no mínimo 1 letra maíscula, 1 número e 1 caracter especial", "Senha");
            }
            return null;
        }
        public static Boolean VerificaSenhaForte(string senha)
        {
            //verifica tamano da senha
            if (senha.Length < 8 || senha.Length > 15)
                return false;
            //verifica se é um numero
            if (!senha.Any(c => char.IsDigit(c)))
                return false;
            //verifica se tem letra maiuscula
            if (!senha.Any(c => char.IsUpper(c)))
                return false;
            //verifica se tem letras minusculas
            if (!senha.Any(c => char.IsLower(c)))
                return false;
            //verifica se tem um caractere espacial
            //if (!senha.Any(c => char.IsSymbol(c)))
            //    return false;

            return true;
        }
    }

}
