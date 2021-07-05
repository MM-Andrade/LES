using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;
using Dominio.Usuarios;
using System.Linq;

namespace Core.Negocio.StUsuarios
{
    public class StAlterarDadosUsuario : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Usuario usuario = (Usuario)entidade;
            var obj = (Usuario)entidade;


            if (usuario.StrAcao == "EditarUsuario")
            {//o usuário tem um ID e a ação é de AlterarDados ? - SIM
      
                Resultado resultado = new Fachada().Consultar(usuario);
                if (string.IsNullOrEmpty(resultado.Mensagem))
                {//retorno vazio ou nulo? sim...

                    obj = (Usuario)resultado.Entidades.FirstOrDefault(); //atribui a entidade a um objeto para utilizar

                    if (usuario.Id != obj.Id)
                    {//a função é administradora ou o ID do usuário que quer alterar é o mesmo que está no BD? - Não
                        return "As Você não tem permissão para alterar esses dados...";
                    }
                    else if(obj.Funcao.Id == 1)
                    {
                        return "As Você não tem permissão para alterar esses dados...";
                    }
                    else if (usuario.Id == obj.Id || obj.Funcao.Id == 1)
                    {//permissão pra alterar OK ? 
                        //sim

                        return null;
                    }
                }
            }// (usuario.Id >= 0 && usuario.StrAcao == "EditarUsuario")

            else
            {   //não 
                if (usuario.StrAcao == "AlterarSenha")        // é só uma alteração de senha...?
                {//sim
                    usuario.StrAcao = "Login"; //para pesquisa do usuário...

                    if (usuario.NovaSenha != usuario.ConfirmarSenha)
                    { //os campos de senha e confirmar senha são iguais? - Não...
                        return "As senhas não conferem...";
                    }

                    //pesquisa o usuário no banco usando ação de "Login"
                    Resultado resultado = new Fachada().Consultar(usuario);

                    if (string.IsNullOrEmpty(resultado.Mensagem))
                    {//string vazia? Sim...

                        obj = (Usuario)resultado.Entidades.FirstOrDefault(); // atribui valor ao objeto para utilizar o retorno dos dados...

                        if (obj.Senha != usuario.Senha)
                        {//senha do usuario é igual a senha que está no bd? - Não...
                            return "A senha atual está incorreta!";
                        }
                        else
                        {//sim, então atribui a ação de alterar senha e manda atualizar...
                            usuario.StrAcao = "AlterarSenha";
                            return null;
                        }
                    }
                    else
                    {// contém informações no "resultado.mensagem"...
                        return "Erro: " + resultado.Mensagem;
                    }
                }
            }
            return "Erro na regra ..."; //..
        }
    }
}
