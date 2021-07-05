using Core.Aplicacao;
using Core.Util;
using Dominio;
using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class FuncaoDAO : AbstractDAO
    {
        public FuncaoDAO() : base("tb_funcao","id")
        {

        }
        public FuncaoDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public FuncaoDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
             base.Atualizar(entidade);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {

            List<Funcao> funcoes = new List<Funcao>();

            try
            {
                OpenConnection();
                DbCommand cmd = ConexaoDB.GetDbCommand(dbConnection);

              
                cmd.CommandText = string.Format("SELECT id,nomefuncao from tb_funcao ORDER BY nomefuncao");

                DbDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows) throw new Exception();

                while (dr.Read())
                {
                    Funcao f = new Funcao()
                    {
                        Id = Convert.ToInt32(dr["id"].ToString()),
                        NomeFuncao = dr["nomefuncao"].ToString(),
                       
                    };

                    funcoes.Add(f);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return funcoes.ToList<IEntidade>();
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
