using Core.Aplicacao;
using Dominio;
using Dominio.Usuarios;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class UsuarioDAO : AbstractDAO
    {

        public UsuarioDAO() : base("tb_usuario", "id")
        {

        }

        public UsuarioDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public UsuarioDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
            if (entidade == null)
                throw new Exception("Entidade nula");

            Usuario usuario = (Usuario)entidade;
            resultado = new Resultado();

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);

                if (usuario.StrAcao == "AlterarSenha")
                {//é uma alteração de senha?
                    cmd.CommandText = $"UPDATE tb_usuario SET senha = '{usuario.NovaSenha}' " +
                        $"WHERE tb_usuario.email = '{usuario.Email}'";

                    cmd.ExecuteNonQuery();
                    Commit();
                }
                else if (usuario.StrAcao == "EditarUsuario")
                {//é uma atualização de informação ?
                    cmd.CommandText = $"UPDATE tb_usuario " +
                        $"SET nome = '{usuario.Nome}',sobrenome = '{usuario.Sobrenome}'," +
                        $"genero = '{usuario.Genero}',email = '{usuario.Email}',senha = '{usuario.Senha}'," +
                        $"ativo = '{usuario.UsuarioAtivo}',fk_funcao = '{usuario.Funcao.Id}'," +
                        $"fk_grupoatendimento = '{usuario.GrupoAtendimento.Id}' " +
                        $"WHERE tb_usuario.id = '{usuario.Id}' RETURNING id";

                    usuario.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    Commit();

                    //instancia o DAO para atualizar endereço...
                    //feito aqui dentro para economizar memória e não chamar o DAO sem necessidade la em cima...
                    EnderecoDAO enderecoDAO = new EnderecoDAO();

                    enderecoDAO.Atualizar(usuario);
                }


            }
            catch (Exception ex)
            {
                Rollback();
                CloseConnection();
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Usuario usuario = entidade as Usuario;
            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                OpenConnection();
                DbCommand cmd = GetDbCommand(dbConnection);


                if (string.IsNullOrEmpty(usuario.StrBusca))             //verifica se está sendo pesquisado algo do usuario
                {//não, string vazia
                    if (usuario.StrAcao == "Login")//tentativa de login ? 
                    {//sim 

                        cmd.CommandText = $"SELECT tb_usuario.id AS idusuario,tb_usuario.nome,tb_usuario.sobrenome,tb_usuario.datanascimento,tb_usuario.datacadastro,tb_usuario.genero,tb_usuario.email,tb_usuario.ativo,tb_usuario.senha,tb_funcao.id AS idfuncao,tb_funcao.nomefuncao,tb_grupoatendimento.id AS idgrupoatendimento,tb_grupoatendimento.nomegrupo,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento,tb_endereco.cidade,tb_endereco.rua,tb_endereco.cep,tb_endereco.estado,tb_endereco.bairro,tb_endereco.complemento,tb_endereco.numero,tb_endereco.cep " +
                            $"FROM tb_usuario " +
                            $"INNER JOIN tb_funcao ON tb_usuario.fk_funcao = tb_funcao.id " +
                            $"INNER JOIN tb_grupoatendimento ON tb_usuario.fk_grupoatendimento = tb_grupoatendimento.id " +
                            $"INNER JOIN tb_departamento ON tb_grupoatendimento.fk_departamento = tb_departamento.id " +
                            $"INNER JOIN tb_endereco ON tb_endereco.fk_usuario = tb_usuario.id " +
                            $"WHERE tb_usuario.email ILIKE '{usuario.Email}' AND tb_usuario.senha LIKE '{usuario.Senha}' AND tb_usuario.ativo = TRUE ";


                        DbDataReader dr = cmd.ExecuteReader();
                        usuarios = DataReaderInfoEditar(dr);
                    }
                    else
                    {//não, é outra ação...
                        if (usuario.Id == 0) //foi passado id do usuário?
                        { // não, seleciona todos
                            cmd.CommandText = "SELECT tb_usuario.id AS idusuario,tb_usuario.nome,tb_usuario.sobrenome,tb_usuario.datanascimento,tb_usuario.datacadastro,tb_usuario.genero,tb_usuario.email,tb_usuario.ativo,tb_funcao.id AS idfuncao,tb_funcao.nomefuncao,tb_grupoatendimento.id AS idgrupoatendimento,tb_grupoatendimento.nomegrupo,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento,tb_endereco.cidade,tb_endereco.rua,tb_endereco.cep,tb_endereco.estado,tb_endereco.bairro,tb_endereco.complemento,tb_endereco.numero,tb_endereco.cep " +
                            "FROM tb_usuario " +
                            "INNER JOIN tb_funcao ON tb_usuario.fk_funcao = tb_funcao.id " +
                            "INNER JOIN tb_grupoatendimento ON tb_usuario.fk_grupoatendimento = tb_grupoatendimento.id " +
                            "INNER JOIN tb_departamento ON tb_grupoatendimento.fk_departamento = tb_departamento.id " +
                            "INNER JOIN tb_endereco ON tb_endereco.fk_usuario = tb_usuario.id ORDER BY tb_usuario.id ASC";

                            DbDataReader dr = cmd.ExecuteReader();
                            usuarios = DataReaderGenerico(dr);
                        }
                        else
                        {   //sim, seleciona o registro enviando o ID
                            cmd.CommandText = $"SELECT tb_usuario.id AS idusuario,tb_usuario.nome,tb_usuario.sobrenome,tb_usuario.datanascimento,tb_usuario.datacadastro,tb_usuario.genero,tb_usuario.email,tb_usuario.ativo,tb_funcao.id AS idfuncao,tb_funcao.nomefuncao,tb_usuario.senha, tb_grupoatendimento.id AS idgrupoatendimento,tb_grupoatendimento.nomegrupo,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento,tb_endereco.cidade,tb_endereco.rua,tb_endereco.cep,tb_endereco.estado,tb_endereco.bairro,tb_endereco.complemento,tb_endereco.numero,tb_endereco.cep " +
                            $"FROM tb_usuario " +
                            $"INNER JOIN tb_funcao ON tb_usuario.fk_funcao = tb_funcao.id " +
                            $"INNER JOIN tb_grupoatendimento ON tb_usuario.fk_grupoatendimento = tb_grupoatendimento.id " +
                            $"INNER JOIN tb_departamento ON tb_grupoatendimento.fk_departamento = tb_departamento.id " +
                            $"INNER JOIN tb_endereco ON tb_endereco.fk_usuario = tb_usuario.id " +
                            $"WHERE tb_usuario.id = '{usuario.Id}'";

                            DbDataReader dr = cmd.ExecuteReader();
                            usuarios = DataReaderInfoEditar(dr);
                        }
                    }
                }
                else
                {//não, string de busca contém algo...
                    cmd.CommandText = string.Format("SELECT tb_usuario.id AS idusuario,tb_usuario.nome,tb_usuario.sobrenome,tb_usuario.datanascimento,tb_usuario.datacadastro,tb_usuario.genero,tb_usuario.email,tb_usuario.ativo,tb_funcao.id AS idfuncao,tb_funcao.nomefuncao,tb_grupoatendimento.id AS idgrupoatendimento,tb_grupoatendimento.nomegrupo,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento,tb_endereco.cidade,tb_endereco.rua,tb_endereco.cep,tb_endereco.estado,tb_endereco.bairro,tb_endereco.complemento,tb_endereco.numero,tb_endereco.cep " +
                            "FROM tb_usuario " +
                            "INNER JOIN tb_funcao ON tb_usuario.fk_funcao = tb_funcao.id " +
                            "INNER JOIN tb_grupoatendimento ON tb_usuario.fk_grupoatendimento = tb_grupoatendimento.id " +
                            "INNER JOIN tb_departamento ON tb_grupoatendimento.fk_departamento = tb_departamento.id " +
                            "INNER JOIN tb_endereco ON tb_endereco.fk_usuario = tb_usuario.id " +
                            "WHERE tb_usuario.nome ILIKE '%{0}%' " +
                            "OR tb_usuario.sobrenome ILIKE '%{0}%' " +
                            "OR tb_usuario.genero ILIKE '%{0}%' " +
                            "OR tb_usuario.email ILIKE '%{0}%' " +
                            "OR tb_funcao.nomefuncao ILIKE '%{0}%' " +
                            "OR tb_departamento.nomedepartamento ILIKE '%{0}%' ", usuario.StrBusca);

                    DbDataReader dr = cmd.ExecuteReader();
                    usuarios = DataReaderGenerico(dr);
                }

            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return usuarios.ToList<IEntidade>();
        }

        public override void Inserir(EntidadeDominio entidade)
        {

            Usuario usuario = (Usuario)entidade;
            resultado = new Resultado();

            EnderecoDAO enderecoDAO = new EnderecoDAO();
            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;


                cmd.CommandText = $"INSERT INTO tb_usuario (nome,sobrenome,datanascimento,datacadastro,genero,email,senha,ativo,fk_funcao,fk_grupoatendimento) VALUES ('{usuario.Nome}','{usuario.Sobrenome}','{usuario.DataNascimento}','{usuario.DataCadastro}','{usuario.Genero}','{usuario.Email}','{usuario.Senha}','{usuario.UsuarioAtivo}','{usuario.Funcao.Id}','{usuario.GrupoAtendimento.Id}') RETURNING id";

                usuario.Id = Convert.ToInt32(cmd.ExecuteScalar());
                Commit();

                enderecoDAO.Inserir(usuario);

            }
            catch (NpgsqlException nex)
            {
                Rollback();
                throw new Exception(resultado.Mensagem + " " + nex.ToString());
            }
            catch (Exception ex)
            {
                Rollback();
                throw new Exception(resultado.Mensagem + " " + ex.ToString());
            }
        }

        public List<Usuario> DataReaderGenerico(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Registro não encontrado");

            List<Usuario> usuarios = new List<Usuario>();

            while (dr.Read())
            {
                Usuario u = new Usuario();
                u.Id = Convert.ToInt32(dr["idusuario"].ToString());
                u.Nome = dr["nome"].ToString();
                u.Sobrenome = dr["sobrenome"].ToString();
                u.DataNascimento = DateTime.Parse(dr["datanascimento"].ToString());
                u.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());
                u.Genero = dr["genero"].ToString();
                u.Email = dr["email"].ToString();
                u.UsuarioAtivo = Convert.ToBoolean(dr["ativo"].ToString());
                u.Funcao.Id = Convert.ToInt32(dr["idfuncao"].ToString());
                u.Funcao.NomeFuncao = dr["nomefuncao"].ToString();
                u.GrupoAtendimento.Id = Convert.ToInt32(dr["idgrupoatendimento"].ToString());
                u.GrupoAtendimento.NomeGrupo = dr["nomegrupo"].ToString();
                u.GrupoAtendimento.Departamento.Id = Convert.ToInt32(dr["iddepartamento"].ToString());
                u.GrupoAtendimento.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();
                u.Endereco.Rua = dr["rua"].ToString();
                u.Endereco.Bairro = dr["bairro"].ToString();
                u.Endereco.Cidade = dr["cidade"].ToString();
                u.Endereco.Estado = dr["estado"].ToString();
                u.Endereco.Numero = dr["numero"].ToString();
                u.Endereco.Complemento = dr["complemento"].ToString();
                u.Endereco.Cep = dr["cep"].ToString();

                usuarios.Add(u);
            }
            dr.Close();

            return usuarios;
        }

        public List<Usuario> DataReaderInfoEditar(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Registro não encontrado");

            List<Usuario> usuarios = new List<Usuario>();

            while (dr.Read())
            {
                Usuario u = new Usuario();
                u.Id = Convert.ToInt32(dr["idusuario"].ToString());
                u.Nome = dr["nome"].ToString();
                u.Sobrenome = dr["sobrenome"].ToString();
                u.DataNascimento = DateTime.Parse(dr["datanascimento"].ToString());
                u.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());
                u.Genero = dr["genero"].ToString();
                u.Email = dr["email"].ToString();
                u.UsuarioAtivo = Convert.ToBoolean(dr["ativo"].ToString());
                u.Funcao.Id = Convert.ToInt32(dr["idfuncao"].ToString());
                u.Funcao.NomeFuncao = dr["nomefuncao"].ToString();
                u.GrupoAtendimento.Id = Convert.ToInt32(dr["idgrupoatendimento"].ToString());
                u.GrupoAtendimento.NomeGrupo = dr["nomegrupo"].ToString();
                u.GrupoAtendimento.Departamento.Id = Convert.ToInt32(dr["iddepartamento"].ToString());
                u.GrupoAtendimento.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();
                u.Endereco.Rua = dr["rua"].ToString();
                u.Endereco.Bairro = dr["bairro"].ToString();
                u.Endereco.Cidade = dr["cidade"].ToString();
                u.Endereco.Estado = dr["estado"].ToString();
                u.Endereco.Numero = dr["numero"].ToString();
                u.Endereco.Complemento = dr["complemento"].ToString();
                u.Endereco.Cep = dr["cep"].ToString();

                //só para ter o campo senha...
                u.Senha = dr["senha"].ToString();

                usuarios.Add(u);
            }
            dr.Close();

            return usuarios;
        }
    }
}
