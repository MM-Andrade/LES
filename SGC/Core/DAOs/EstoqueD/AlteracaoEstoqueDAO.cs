using Dominio;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs.EstoqueD
{
    public class AlteracaoEstoqueDAO : AbstractDAO
    {
        public AlteracaoEstoqueDAO() : base("tb_alteracaoestoque", "id")
        {

        }
        public AlteracaoEstoqueDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public AlteracaoEstoqueDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            AlteracaoEstoque estoque = entidade as AlteracaoEstoque;
            List<AlteracaoEstoque> itensestoque = new List<AlteracaoEstoque>();

            try
            {
                OpenConnection();
                DbCommand cmd = GetDbCommand(dbConnection);

                if (!string.IsNullOrEmpty(estoque.Motivo))
                {

                    cmd.CommandText = $"SELECT tb_estoque.item " +
                        $"FROM tb_estoque " +
                        $"INNER JOIN tb_alteracaoestoque ON tb_alteracaoestoque.fk_estoque = tb_estoque.id " +
                        $"WHERE tb_alteracaoestoque.motivo LIKE '{estoque.Motivo}'";


                    DbDataReader dr = cmd.ExecuteReader();
                    itensestoque = DatareaderEstoqueGenerico(dr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }

            return itensestoque.ToList<IEntidade>();
        }

        public override void Inserir(EntidadeDominio entidade)
        {
            AlteracaoEstoque estoque = entidade as AlteracaoEstoque;

            try
            {
                if (dbConnection == null)
                {
                    OpenConnection();
                    BeginTransaction();
                }
                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;
                if (estoque.Estoque.StrAcao == "EditarProduto")
                {
                    cmd.CommandText = $"INSERT INTO tb_alteracaoestoque (datacadastro,fk_estoque,fk_usuario) VALUES ('{estoque.DataCadastro}','{estoque.Estoque.Id}','{estoque.Estoque.GetUsuario.Id}')";
                    cmd.ExecuteNonQuery();

                    Commit();
                }
                else
                {
                    cmd.CommandText = $"INSERT INTO tb_alteracaoestoque (motivo,datacadastro,acao,qtdebaixa,fk_estoque,fk_usuario) VALUES ('{estoque.Estoque.GetTicket.CodigoTicket}','{estoque.DataCadastro}','{estoque.Acao}','{estoque.QuantidadeBaixa}','{estoque.Estoque.Id}','{estoque.Estoque.GetTicket.Usuario.Id}')";
                    cmd.ExecuteNonQuery();

                    Commit();
                }

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

        private List<AlteracaoEstoque> DatareaderEstoqueGenerico(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Sem itens utilizados");

            List<AlteracaoEstoque> itensestoque = new List<AlteracaoEstoque>();

            while (dr.Read())
            {
                AlteracaoEstoque est = new AlteracaoEstoque();

                est.Estoque.Item = dr["item"].ToString();

                itensestoque.Add(est);
            }
            dr.Close();

            return itensestoque;
        }
    }
}

