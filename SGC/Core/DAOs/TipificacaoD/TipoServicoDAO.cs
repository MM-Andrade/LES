using Core.Aplicacao;
using Dominio;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class TipoServicoDAO : AbstractDAO
    {
        public TipoServicoDAO() : base("tb_tiposervico", "id")
        {

        }
        public TipoServicoDAO(string table, string idTable) : base(table, idTable)
        {
        }
        public TipoServicoDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
            if (entidade == null)
                throw new Exception("Entidade nula");

            TipoServico tipoServico = (TipoServico)entidade;
            resultado = new Resultado();

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                if (string.IsNullOrEmpty(tipoServico.StrAcao))
                {
                    cmd.CommandText = $"UPDATE tb_tiposervico SET status = '{tipoServico.Status}', " +
                        $"nomeservico = '{tipoServico.NomeServico}',fk_departamento = '{tipoServico.Departamento.Id}'," +
                        $"fk_prioridade = '{tipoServico.Prioridade.Id}' " +
                        $" WHERE id = '{tipoServico.Id}'";
                }
                else if (tipoServico.StrAcao == "Desativar")
                {
                    cmd.CommandText = $"UPDATE tb_tiposervico SET status = '{tipoServico.Status}' WHERE id = '{tipoServico.Id}'";
                }

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
        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            TipoServico tipoServico = entidade as TipoServico;
            List<TipoServico> tipoServicos = new List<TipoServico>();

            try
            {
                OpenConnection();
                DbCommand cmd = GetDbCommand(dbConnection);

                if (string.IsNullOrEmpty(tipoServico.StrBusca))             //verifica se está sendo pesquisado algum serviço especifico
                {//não, string vazia
                    if (tipoServico.Id == 0) //foi passado id do serviço para ser selecionado?
                    { // não, seleciona todos
                        cmd.CommandText = string.Format("SELECT tb_tiposervico.id AS idtiposervico,tb_tiposervico.nomeservico,tb_tiposervico.status,tb_tiposervico.datacadastro,tb_prioridade.id AS idprioridade,tb_prioridade.nivelprioridade,tb_prioridade.descricaoprioridade,tb_prioridade.tempodeatendimento,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento " +
                            "FROM tb_tiposervico " +
                            "INNER JOIN tb_prioridade ON tb_tiposervico.fk_prioridade = tb_prioridade.id " +
                            "INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                            "WHERE tb_tiposervico.status ILIKE 't' ORDER BY tb_departamento.nomedepartamento");
                    }
                    else
                    {   //sim, seleciona o registro enviando o ID
                        cmd.CommandText = $"SELECT tb_tiposervico.id AS idtiposervico,tb_tiposervico.nomeservico,tb_tiposervico.status,tb_tiposervico.datacadastro,tb_prioridade.id AS idprioridade,tb_prioridade.nivelprioridade,tb_prioridade.descricaoprioridade,tb_prioridade.tempodeatendimento,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento " +
                            $"FROM tb_tiposervico " +
                            $"INNER JOIN tb_prioridade ON tb_tiposervico.fk_prioridade = tb_prioridade.id " +
                            $"INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                            $"WHERE tb_tiposervico.status ILIKE 't' AND tb_tiposervico.id = '{tipoServico.Id}' ORDER BY tb_departamento.nomedepartamento";
                    }
                }
                else
                {//não, string de busca contém algo...
                    cmd.CommandText = string.Format("SELECT tb_tiposervico.id AS idtiposervico,tb_tiposervico.nomeservico,tb_tiposervico.status,tb_tiposervico.datacadastro,tb_prioridade.id AS idprioridade,tb_prioridade.nivelprioridade,tb_prioridade.descricaoprioridade,tb_prioridade.tempodeatendimento,tb_departamento.id AS iddepartamento,tb_departamento.nomedepartamento " +
                            "FROM tb_tiposervico " +
                            "INNER JOIN tb_prioridade ON tb_tiposervico.fk_prioridade = tb_prioridade.id " +
                            "INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
              "WHERE tb_tiposervico.nomeservico ILIKE '{0}%'" +
              "OR tb_departamento.nomedepartamento ILIKE '{0}%' " +
              "OR tb_prioridade.descricaoprioridade ILIKE '{0}%' ", tipoServico.StrBusca);
                }

                DbDataReader dr = cmd.ExecuteReader();

                if (!dr.HasRows)
                    throw new Exception("Sem tipiticação para o termo pesquisado");

                while (dr.Read())
                {
                    TipoServico tps = new TipoServico();

                    tps.Id = Convert.ToInt32(dr["idtiposervico"].ToString());
                    tps.NomeServico = dr["nomeservico"].ToString();
                    tps.Status = dr["status"].ToString();
                    tps.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());

                    tps.Prioridade.Id = Convert.ToInt32(dr["idprioridade"].ToString());
                    tps.Prioridade.NivelPrioridade = Convert.ToInt32(dr["nivelprioridade"].ToString());
                    tps.Prioridade.DescricaoPrioridade = dr["descricaoprioridade"].ToString();
                    tps.Prioridade.TempoAtendimento = Convert.ToInt32(dr["tempodeatendimento"].ToString());

                    tps.Departamento.Id = Convert.ToInt32(dr["iddepartamento"].ToString());
                    tps.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();
                    
                    tipoServicos.Add(tps);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tipoServicos.ToList<IEntidade>();
        }
        public override void Excluir(EntidadeDominio entidade)
        {
            base.Excluir(entidade);
        }
        public override void Inserir(EntidadeDominio entidade)
        {
            TipoServico tipoServico = (TipoServico)entidade;
            resultado = new Resultado();

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;


                cmd.CommandText = $"INSERT INTO tb_tiposervico (nomeservico,status,datacadastro,fk_prioridade,fk_departamento) VALUES ('{tipoServico.NomeServico}','{tipoServico.Status}','{tipoServico.DataCadastro}','{tipoServico.Prioridade.Id}','{tipoServico.Departamento.Id}')";
                
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
