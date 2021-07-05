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
    public class DepartamentoDAO : AbstractDAO
    {
        public DepartamentoDAO() : base("tb_departamento", "id")
        {

        }
        public DepartamentoDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public DepartamentoDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
            base.Atualizar(entidade);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            List<Departamento> departamentos = new List<Departamento>();

            try
            {
                OpenConnection();
                DbCommand cmd = ConexaoDB.GetDbCommand(dbConnection);

                cmd.CommandText = string.Format("SELECT id,nomedepartamento from tb_departamento ORDER BY nomedepartamento");

                DbDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows) throw new Exception();

                while (dr.Read())
                {
                    Departamento d = new Departamento()
                    {
                        Id = Convert.ToInt32(dr["id"].ToString()),
                        NomeDepartamento = dr["nomedepartamento"].ToString(),

                    };

                    departamentos.Add(d);
                }
                dr.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return departamentos.ToList<IEntidade>();
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
