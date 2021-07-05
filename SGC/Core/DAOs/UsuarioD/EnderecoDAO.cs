using Core.Aplicacao;
using Dominio;
using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Core.DAOs
{
    public class EnderecoDAO : AbstractDAO
    {
        public EnderecoDAO() : base("tb_endereco", "id")
        {

        }
        public EnderecoDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public EnderecoDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
            if (entidade == null)
                throw new Exception("Entidade Nula");

            Usuario usuario = entidade as Usuario;
            resultado = new Resultado();

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = $"UPDATE tb_endereco " +
                    $"SET rua = '{usuario.Endereco.Rua}',cidade = '{usuario.Endereco.Cidade}'," +
                    $"bairro = '{usuario.Endereco.Bairro}',estado = '{usuario.Endereco.Estado}'," +
                    $"cep = '{usuario.Endereco.Cep}',complemento = '{usuario.Endereco.Complemento}'," +
                    $"numero = '{usuario.Endereco.Numero}' WHERE fk_usuario = '{usuario.Id}'";

                cmd.ExecuteNonQuery();
                Commit();

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
            return base.Consultar(entidade);
        }

        public override void Excluir(EntidadeDominio entidade)
        {
            base.Excluir(entidade);
        }

        public override void Inserir(EntidadeDominio entidade)
        {

            Usuario usuario = entidade as Usuario;

            resultado = new Resultado();

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = $"INSERT INTO tb_endereco(rua,cidade,bairro,estado,cep,complemento,numero,fk_usuario) VALUES " +
                    $"('{usuario.Endereco.Rua}','{usuario.Endereco.Cidade}','{usuario.Endereco.Bairro}','{usuario.Endereco.Estado}','{usuario.Endereco.Cep}'," +
                    $"'{usuario.Endereco.Complemento}','{usuario.Endereco.Numero}','{usuario.Id}')";

                cmd.ExecuteNonQuery();

                Commit();

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
    }
}
