using Core.Aplicacao;
using Dominio;
using Dominio.Tipificacao;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs.Tipificacao
{
    class ItemServicoDAO : AbstractDAO
    {
        public ItemServicoDAO() : base("id", "tb_itemservico")
        {

        }

        public ItemServicoDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public ItemServicoDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            ItemServico itemServico = entidade as ItemServico;
            List<ItemServico> lstItemServico = new List<ItemServico>();

            try
            {
                OpenConnection();
                DbCommand cmd = GetDbCommand(dbConnection);

                if (string.IsNullOrEmpty(itemServico.StrBusca))
                { //busca vazia ? - sim


                    if (itemServico.Id <= 0 && itemServico.Dpto.Id <= 0)
                    {//sim, seleciona todos
                        cmd.CommandText = $"SELECT tb_itemservico.id AS iditemservico,tb_itemservico.itemservico,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento " +
                            $"FROM tb_itemservico " +
                            $"INNER JOIN tb_departamento ON tb_itemservico.fk_departamento = tb_departamento.id "+
                            "ORDER BY tb_itemservico.id";

                        DbDataReader dr = cmd.ExecuteReader();
                        lstItemServico = DatareaderServicos(dr);
                    }
                    else if(itemServico.Dpto.Id > 0)
                    {//pesquisa para resolver chamado
                        cmd.CommandText = $"SELECT tb_itemservico.id AS iditemservico,tb_itemservico.itemservico,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento " +
                            $"FROM tb_itemservico " +
                            $"INNER JOIN tb_departamento ON tb_itemservico.fk_departamento = tb_departamento.id " +
                            $"WHERE tb_departamento.id = '{itemServico.Dpto.Id}'";

                        DbDataReader dr = cmd.ExecuteReader();
                        lstItemServico = DatareaderServicos(dr);

                    }
                    else
                    {//não, estou passando um ID
                        cmd.CommandText = $"SELECT tb_itemservico.id AS iditemservico,tb_itemservico.itemservico,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento " +
                            $"FROM tb_itemservico " +
                            $"INNER JOIN tb_departamento ON tb_itemservico.fk_departamento = tb_departamento.id " +
                            $"WHERE tb_itemservico.id = '{itemServico.Id}'";

                        DbDataReader dr = cmd.ExecuteReader();
                        lstItemServico = DatareaderServicos(dr);
                    }
                }
                else
                {//- não...
                    cmd.CommandText = $"SELECT tb_itemservico.id AS iditemservico,tb_itemservico.itemservico,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento " +
                            $"FROM tb_itemservico " +
                            $"INNER JOIN tb_departamento ON tb_itemservico.fk_departamento = tb_departamento.id " +
                        $"WHERE itemservico ILIKE '%{itemServico.StrBusca}%' " +
                        $"OR tb_departamento.nomedepartamento ILIKE '%{itemServico.StrBusca}%'";

                    DbDataReader dr = cmd.ExecuteReader();
                    lstItemServico = DatareaderServicos(dr);
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

            return lstItemServico.ToList<IEntidade>();
        }

        public override void Inserir(EntidadeDominio entidade)
        {
            ItemServico itemServico = entidade as ItemServico;

            Resultado resultado = new Resultado();

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = $"INSERT INTO tb_itemservico (itemservico,fk_departamento) VALUES ('{itemServico.Item}','{itemServico.Dpto.Id}')";
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

        public List<ItemServico> DatareaderServicos(DbDataReader dr)
        {

            if (!dr.HasRows)
                throw new Exception("Sem itens cadastrados");

            List<ItemServico> itens = new List<ItemServico>();

            while (dr.Read())
            {
                ItemServico item = new ItemServico();

                item.Id = Convert.ToInt32(dr["iditemservico"].ToString());
                item.Item = dr["itemservico"].ToString();
                item.Dpto.NomeDepartamento = dr["nomedepartamento"].ToString();

                itens.Add(item);
            }
            dr.Close();

            return itens;

        }

        public override void Excluir(EntidadeDominio entidade)
        {
            ItemServico itemServico = (ItemServico)entidade;
            resultado = new Resultado();

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;


                cmd.CommandText = $"DELETE FROM tb_itemservico WHERE tb_itemservico.id = {itemServico.Id}";

                cmd.ExecuteNonQuery();
                Commit();
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
    }
}
