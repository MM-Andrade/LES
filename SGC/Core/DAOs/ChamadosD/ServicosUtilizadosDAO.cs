using Core.Aplicacao;
using Dominio;
using Dominio.Chamados;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class ServicosUtilizadosDAO : AbstractDAO
    {
        public ServicosUtilizadosDAO() : base ("tb_servicosutilizados","id")
        {

        }
        public ServicosUtilizadosDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public ServicosUtilizadosDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {

            if (entidade == null)
            {
                return null;
            }

            ServicosUtilizados servicosUtilizados = (ServicosUtilizados)entidade;
            resultado = new Resultado();

            List<ServicosUtilizados> lstservicosUtilizados = new List<ServicosUtilizados>();

            try
            {
                OpenConnection();

                DbCommand cmd = GetDbCommand(dbConnection);

                cmd.CommandType = CommandType.Text;
                if (servicosUtilizados.GetTicket.Id == 0)
                {
                    throw new Exception("Sem ticket informado");
                }
                else
                {
                    cmd.CommandText = $"SELECT tb_itemservico.itemservico " +
                        $"FROM tb_servicosutilizados " +
                        $"INNER JOIN tb_itemservico ON tb_servicosutilizados.fk_itemservico = tb_itemservico.id " +
                        $"INNER JOIN tb_ticket ON tb_servicosutilizados.fk_ticket = tb_ticket.id " +
                        $"WHERE tb_ticket.id = '{servicosUtilizados.GetTicket.Id}'";
                }
                DbDataReader dr = cmd.ExecuteReader();
                lstservicosUtilizados = DataReaderServico(dr);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return lstservicosUtilizados.ToList<IEntidade>();
        }
        public override void Inserir(EntidadeDominio entidade)
        {
            ServicosUtilizados servicosUtilizados = (ServicosUtilizados)entidade;
            
            try
            {
                if(dbConnection == null)
                {
                    OpenConnection();
                    BeginTransaction();
                }

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = $"INSERT INTO tb_servicosutilizados (fk_ticket,fk_itemservico)" +
                    $" VALUES ('{servicosUtilizados.GetTicket.Id}','{servicosUtilizados.GetItemServico.Id}')";

                cmd.ExecuteNonQuery();
                Commit();

            }
            catch (NpgsqlException nex)
            {
                Rollback();
                throw nex;
            }
            catch (Exception ex)
            {
                Rollback();
                throw ex;
            }
        }

        private List<ServicosUtilizados> DataReaderServico(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Sem serviços...");

            List<ServicosUtilizados> servicosUtilizados = new List<ServicosUtilizados>();

            while (dr.Read())
            {
                ServicosUtilizados servicos = new ServicosUtilizados();

                servicos.GetTicket.TipoServico.NomeServico = dr["itemservico"].ToString();


                servicosUtilizados.Add(servicos);
            }
            dr.Close();

            return servicosUtilizados;
        }
    }
}
