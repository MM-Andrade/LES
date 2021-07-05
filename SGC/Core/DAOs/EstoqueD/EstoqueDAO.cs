using Core.Aplicacao;
using Core.Controle;
using Core.DAOs.EstoqueD;
using Dominio;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class EstoqueDAO : AbstractDAO
    {
        public EstoqueDAO() : base("tb_estoque", "id")
        {

        }
        public EstoqueDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public EstoqueDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
            Estoque estoque = (Estoque)entidade;
            AlteracaoEstoque alteracaoEstoque = new AlteracaoEstoque();
            resultado = new Resultado();

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                if(estoque.GetTicket.StrAcao == "ResolverChamado")
                {
                    cmd.CommandText = $"UPDATE tb_estoque SET quantidadeestoque = '{estoque.QuantidadeEstoque}' " +
                        $"WHERE tb_estoque.id = '{estoque.Id}' RETURNING id";

                    estoque.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    Commit();

                    alteracaoEstoque.Estoque = estoque;
                    resultado = new Fachada().Inserir(alteracaoEstoque);
                }
                else if(estoque.StrAcao == "EditarProduto")
                {
                    cmd.CommandText = $"UPDATE tb_estoque SET quantidadeestoque = '{estoque.QuantidadeEstoque}' " +
                       $"WHERE tb_estoque.id = '{estoque.Id}' RETURNING id";

                    estoque.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    Commit();

                    alteracaoEstoque.Estoque = estoque;
                    resultado = new Fachada().Inserir(alteracaoEstoque);
                }

            }
            catch (Exception ex)
            {
                Rollback();
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Estoque estoque = entidade as Estoque;
            List<Estoque> itensestoque = new List<Estoque>();

            try
            {
                OpenConnection();
                DbCommand cmd = GetDbCommand(dbConnection);

                if (string.IsNullOrEmpty(estoque.StrBusca))
                { //busca vazia ? - sim


                    if (estoque.Id <= 0 && estoque.Departamento.Id <= 0)
                    {//sim, seleciona todos
                        cmd.CommandText = "SELECT tb_estoque.id AS idestoque,tb_estoque.item,tb_estoque.quantidadeestoque,tb_estoque.unidade,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento,tb_estoque.codigoean " +
                            "FROM tb_estoque " +
                            "INNER JOIN tb_departamento ON tb_estoque.fk_departamento = tb_departamento.id " +
                            "ORDER BY tb_estoque.id";

                        DbDataReader dr = cmd.ExecuteReader();
                        itensestoque = DatareaderEstoqueGenerico(dr);
                    }

                    else if(estoque.Departamento.Id > 0)
                    {//consulta do item pra resolução
                        cmd.CommandText = $"SELECT tb_estoque.id AS idestoque,tb_estoque.item,tb_estoque.quantidadeestoque,tb_estoque.unidade,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento,tb_estoque.codigoean  " +
                            $"FROM tb_estoque " +
                            $"INNER JOIN tb_departamento ON tb_estoque.fk_departamento = tb_departamento.id " +
                            $"WHERE tb_departamento.id = '{estoque.Departamento.Id}' AND tb_estoque.quantidadeestoque > 0 " +
                            $"ORDER BY tb_estoque.id";

                        DbDataReader dr = cmd.ExecuteReader();
                        itensestoque = DatareaderEstoqueGenerico(dr);
                    }

                    else if (estoque.Id > 0)
                    {//não, estou passando um ID
                        cmd.CommandText = $"SELECT tb_estoque.id AS idestoque,tb_estoque.item,tb_estoque.quantidadeestoque,tb_estoque.unidade,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento,tb_estoque.codigoean  " +
                            $"FROM tb_estoque " +
                            $"INNER JOIN tb_departamento ON tb_estoque.fk_departamento = tb_departamento.id " +
                            $"WHERE tb_estoque.id = '{estoque.Id}'";

                        DbDataReader dr = cmd.ExecuteReader();
                        itensestoque = DatareaderEstoqueGenerico(dr);
                    }
                }
                else
                {//- não... vou buscar algo...
                    cmd.CommandText = $"SELECT tb_estoque.id AS idestoque,tb_estoque.item,tb_estoque.quantidadeestoque,tb_estoque.unidade,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento,tb_estoque.codigoean  " +
                            $"FROM tb_estoque " +
                            $"INNER JOIN tb_departamento ON tb_estoque.fk_departamento = tb_departamento.id " +
                        $"WHERE item ILIKE '%{estoque.StrBusca}%' " +
                        $"OR tb_departamento.nomedepartamento ILIKE '%{estoque.StrBusca}%' " +
                        $"OR tb_estoque.codigoean ILIKE '%{estoque.StrBusca}%'";

                    DbDataReader dr = cmd.ExecuteReader();
                    itensestoque = DatareaderEstoqueGenerico(dr);
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

            return itensestoque.ToList<IEntidade>();
        }

        public override void Excluir(EntidadeDominio entidade)
        {
            base.Excluir(entidade);
        }

        public override void Inserir(EntidadeDominio entidade)
        {
            Estoque estoque = entidade as Estoque;

            Resultado resultado = new Resultado();

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = $"INSERT INTO tb_estoque (item,quantidadeestoque,unidade,fk_departamento,codigoean) VALUES ('{estoque.Item}','{estoque.QuantidadeEstoque}','{estoque.Unidade}','{estoque.Departamento.Id}','{estoque.codigoean}')";
                cmd.ExecuteNonQuery();

                Commit();


            }
            catch (NpgsqlException nex)
            {
                Rollback();
                CloseConnection();
                throw new Exception(resultado.Mensagem + " " + nex.ToString());

            }
            catch (Exception ex)
            {
                Rollback();
                CloseConnection();
                throw new Exception(resultado.Mensagem + " " + ex.ToString());
            }
            finally
            {
                CloseConnection();
            }
        }

        public List<Estoque> DatareaderEstoqueGenerico(DbDataReader dr)
        {

            if (!dr.HasRows)
                throw new Exception("Sem itens cadastrados");

            List<Estoque> itensestoque = new List<Estoque>();

            while (dr.Read())
            {
                Estoque est = new Estoque();

                est.Id = Convert.ToInt32(dr["idestoque"].ToString());
                est.Item = dr["item"].ToString();
                est.QuantidadeEstoque = Convert.ToInt32(dr["quantidadeestoque"].ToString());
                est.Unidade = dr["unidade"].ToString();
                est.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();
                est.Departamento.Id = Convert.ToInt32(dr["iddepartamento"].ToString());
                est.codigoean = dr["codigoean"].ToString();

                itensestoque.Add(est);
            }
            dr.Close();

            return itensestoque;
        }
    }
}
