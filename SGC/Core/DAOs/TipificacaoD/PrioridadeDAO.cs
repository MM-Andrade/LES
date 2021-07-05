using Core.Aplicacao;
using Core.Util;
using Dominio;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class PrioridadeDAO : AbstractDAO
    {
        public PrioridadeDAO() : base("tb_prioridade","id")
        {

        }

        public PrioridadeDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public PrioridadeDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
            base.Atualizar(entidade);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            List<Prioridade> prioridades = new List<Prioridade>();

            try
            {
                OpenConnection();
                DbCommand cmd = ConexaoDB.GetDbCommand(dbConnection);

                cmd.CommandText = string.Format("SELECT id,nivelprioridade,descricaoprioridade,tempodeatendimento from tb_Prioridade ORDER BY nivelprioridade");

                DbDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows) throw new Exception();

                while (dr.Read())
                {
                    Prioridade pri = new Prioridade()
                    {
                        Id = Convert.ToInt32(dr["id"].ToString()),
                        NivelPrioridade = Convert.ToInt32(dr["nivelprioridade"].ToString()),
                        DescricaoPrioridade = dr["descricaoprioridade"].ToString(),
                        TempoAtendimento = Convert.ToInt32(dr["tempodeatendimento"].ToString())
                    };

                    prioridades.Add(pri);
                }
                dr.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return prioridades.ToList<IEntidade>();
        }

        public override void Excluir(EntidadeDominio entidade)
        {
            base.Excluir(entidade);
        }

        public override void Inserir(EntidadeDominio entidade)
        {
            base.Inserir(entidade);
        }
    }
}
