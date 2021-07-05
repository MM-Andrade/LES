using Core.Aplicacao;
using Dominio;
using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class GrupoAtendimentoDAO : AbstractDAO
    {
        public GrupoAtendimentoDAO() : base("tb_grupoatendimento", "id")
        {

        }

        public GrupoAtendimentoDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public GrupoAtendimentoDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
            base.Atualizar(entidade);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            GrupoAtendimento grupoAtendimento = entidade as GrupoAtendimento;
            List<GrupoAtendimento> gruposAtendimento = new List<GrupoAtendimento>();

            try
            {
                OpenConnection();
                DbCommand cmd = GetDbCommand(dbConnection);


                if (string.IsNullOrEmpty(grupoAtendimento.StrBusca))             //verifica se está sendo pesquisado algo do usuario
                {//não, string vazia
                    if(grupoAtendimento.StrAcao == "CadastroTipoServico")
                    {//sim. Select para cadastrar um tipo de Serviço

                        cmd.CommandText = string.Format("SELECT tb_grupoatendimento.id as idgrupo,tb_grupoatendimento.nomegrupo,tb_departamento.nomedepartamento,tb_departamento.id as iddepartamento " +
                            "FROM tb_grupoatendimento " +
                            "INNER JOIN tb_departamento ON tb_grupoatendimento.fk_departamento = tb_departamento.id ");

                        DbDataReader dr = cmd.ExecuteReader();
                        gruposAtendimento = DataReaderCadastrarServico(dr);

                    }
                    else if (grupoAtendimento.Id <= 0)//foi passado id?
                    { // não, seleciona todos
                        cmd.CommandText = string.Format("SELECT tb_grupoatendimento.id as idgrupo,tb_grupoatendimento.nomegrupo,tb_departamento.nomedepartamento,tb_departamento.id as iddepartamento " +
                            "FROM tb_grupoatendimento " +
                            "INNER JOIN tb_departamento ON tb_grupoatendimento.fk_departamento = tb_departamento.id ");

                        DbDataReader dr = cmd.ExecuteReader();
                        gruposAtendimento = DataReaderCadastrarServico(dr);
                    }
                    else
                    {   //sim, seleciona o registro enviando o ID
                        cmd.CommandText = string.Format("SELECT tb_usuario.nome,tb_usuario.sobrenome,tb_grupoatendimento.nomegrupo,tb_grupoatendimento.id" +
                        "FROM tb_usuario " +
                        "INNER JOIN tb_grupoatendimento ON tb_usuario.fk_grupoatendimento = tb_grupoatendimento.id " +
                        "WHERE tb_grupoatendimento.id = '{0}'", grupoAtendimento.Id);

                        DbDataReader dr = cmd.ExecuteReader();
                        gruposAtendimento = DataReaderGrupoxUsuario(dr);
                    }
                }
                else
                {//não, string de busca contém algo...
                    cmd.CommandText = string.Format("SELECT tb_grupoatendimento.nomegrupo,tb_departamento.nomedepartamento " +
                            "FROM tb_grupoatendimento " +
                            "INNER JOIN tb_departamento ON tb_grupoatendimento.fk_departamento = tb_departamento.id "+
                           "WHERE tb_grupoatendimento.nomegrupo ILIKE '{0}%' " +
                           "OR tb_departamento.nomedepartamento ILIKE '{0}%' ", grupoAtendimento.StrBusca);

                    DbDataReader dr = cmd.ExecuteReader();
                    gruposAtendimento = DataReaderGenerico(dr);
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

            return gruposAtendimento.ToList<IEntidade>();
        }

        public override void Excluir(EntidadeDominio entidade)
        {
            base.Excluir(entidade);
        }

        public override void Inserir(EntidadeDominio entidade)
        {
            base.Inserir(entidade);
        }


        public List<GrupoAtendimento> DataReaderGenerico(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Sem grupos de atendimento");

            List<GrupoAtendimento> gruposAtendimento = new List<GrupoAtendimento>();
            while (dr.Read())
            {
                GrupoAtendimento grupoAtendimento = new GrupoAtendimento();
                grupoAtendimento.Id = Convert.ToInt32(dr["id"].ToString());
                grupoAtendimento.NomeGrupo = dr["nomegrupo"].ToString();
                grupoAtendimento.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();

                gruposAtendimento.Add(grupoAtendimento);
            }
            dr.Close();

            return gruposAtendimento;
        }

        public List<GrupoAtendimento> DataReaderGrupoxUsuario(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Sem grupos de atendimento");

            List<GrupoAtendimento> gruposAtendimento = new List<GrupoAtendimento>();
            while (dr.Read())
            {
                GrupoAtendimento grupoAtendimento = new GrupoAtendimento();
                grupoAtendimento.Id = Convert.ToInt32(dr["id"].ToString());
                grupoAtendimento.NomeGrupo = dr["nomegrupo"].ToString();
                grupoAtendimento.Usuario.Nome = dr["nome"].ToString();
                grupoAtendimento.Usuario.Sobrenome = dr["sobrenome"].ToString();

                gruposAtendimento.Add(grupoAtendimento);
            }
            dr.Close();

            return gruposAtendimento;
        }

        public List<GrupoAtendimento> DataReaderCadastrarServico(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Sem grupos de atendimento");

            List<GrupoAtendimento> gruposAtendimento = new List<GrupoAtendimento>();
            while (dr.Read())
            {
                GrupoAtendimento grupoAtendimento = new GrupoAtendimento();
                grupoAtendimento.Id = Convert.ToInt32(dr["idgrupo"].ToString());
                grupoAtendimento.Departamento.Id = Convert.ToInt32(dr["iddepartamento"].ToString());
                grupoAtendimento.NomeGrupo = dr["nomegrupo"].ToString();
                grupoAtendimento.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();

                gruposAtendimento.Add(grupoAtendimento);
            }
            dr.Close();

            return gruposAtendimento;
        }
    }
}
