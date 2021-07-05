using Core.Aplicacao;
using Core.Core;
using Core.Util;
using Dominio;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Core.DAOs
{
    public abstract class AbstractDAO : ConexaoDB, IDAO
    {


        protected string table, idTable;

        protected new DbConnection dbConnection;

        //Constutores

        public AbstractDAO(string table, string idTable)
        {
            this.table = table;
            this.idTable = idTable;
        }

        public AbstractDAO(DbConnection dbConnection, string table, string idTable)
        {
            this.table = table;
            this.idTable = idTable;
            this.dbConnection = dbConnection;
        }

        public void OpenConnection()
        {
            try
            {
                if(dbConnection == null)
                {
                    dbConnection = GetDbConnection();
                    dbConnection.Open();
                }
            }
            catch (NpgsqlException nex)
            {

                throw nex;
            }
            catch (Exception ex)
            {

                throw ex;
            }
    
        }
        public void CloseConnection()
        {
            if(dbConnection !=null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
        public void BeginTransaction()
        {
            try
            {
                dbTransaction = dbConnection.BeginTransaction();
            }
            catch (DbException dbex)
            {
                dbex.ToString();

                throw dbex;
            }
        }
        public void Rollback()
        {
            try
            {
                dbTransaction.Rollback();
            }
            catch (DbException dbex)
            {
                dbex.ToString();

                throw dbex;
            }
        }
        public void Commit()
        {
            try
            {
                dbTransaction.Commit();
            }
            catch (DbException dbex)
            {
                dbex.ToString();

                throw dbex;
            }

        }

        public virtual void Atualizar(EntidadeDominio entidade)
        {
            throw new NotImplementedException();
        }

        public virtual List<IEntidade> Consultar(IEntidade entidade)
        {
            throw new NotImplementedException();
        }

        public virtual void Excluir(EntidadeDominio entidade)
        {
            throw new NotImplementedException();
        }

        public virtual void Inserir(EntidadeDominio entidade)
        {
            throw new NotImplementedException();
        }
    }
}
