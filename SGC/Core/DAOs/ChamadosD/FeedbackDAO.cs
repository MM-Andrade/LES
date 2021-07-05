using Core.Aplicacao;
using Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class FeedbackDAO : AbstractDAO
    {
        public FeedbackDAO() : base("tb_feedback", "id")
        {

        }

        public FeedbackDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public FeedbackDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {

            Feedback feedback = (Feedback)entidade;

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = $"UPDATE tb_feedback SET " +
        $"status = '{feedback.Status}', notaatendimento = '{feedback.NotaAtendimento}', notaatendente = '{feedback.NotaAtendente}', comentarios = '{feedback.Comentarios}' " +
        $"WHERE tb_feedback.id = '{feedback.Id}'";

                cmd.ExecuteNonQuery();
                Commit();



            }
            catch (Exception ex)
            {
                Rollback();
                CloseConnection();

                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            Feedback feedback = (Feedback)entidade;

            List<Feedback> feedbacks = new List<Feedback>();
            try
            {
                OpenConnection();
                DbCommand cmd = GetDbCommand(dbConnection);

                if (feedback.StrAcao == "ConsultaFeedbackAdm")
                {
                    cmd.CommandText = $"SELECT tb_feedback.id AS idfeedback,tb_feedback.notaatendimento,tb_feedback.notaatendente,tb_feedback.comentarios,tb_feedback.status AS statusfeedback,tb_ticket.codigoticket,tb_ticket.tecnicoresponsavel,tb_feedback.datacadastro,tb_ticket.id AS idticket " +
                        $"FROM tb_feedback " +
                        $"INNER JOIN tb_ticket ON tb_feedback.fk_ticket = tb_ticket.id ORDER BY tb_feedback.datacadastro DESC";
                }
                else if (feedback.StrAcao == "VisualizarFeedback")
                {
                    cmd.CommandText = $"SELECT tb_feedback.id AS idfeedback,tb_feedback.notaatendimento,tb_feedback.notaatendente,tb_feedback.comentarios,tb_feedback.status AS statusfeedback,tb_ticket.codigoticket,tb_ticket.tecnicoresponsavel,tb_feedback.datacadastro,tb_ticket.id AS idticket " +
                          $"FROM tb_feedback " +
                          $"INNER JOIN tb_ticket ON tb_feedback.fk_ticket = tb_ticket.id " +
                          $"WHERE tb_ticket.codigoticket LIKE '{feedback.Ticket.CodigoTicket}'";
                }
                else if (feedback.StrAcao == "ConsultaFeedbackUsuario")
                {
                    cmd.CommandText = $"SELECT tb_feedback.id AS idfeedback,tb_feedback.notaatendimento,tb_feedback.notaatendente,tb_feedback.comentarios,tb_feedback.status AS statusfeedback,tb_ticket.codigoticket,tb_ticket.tecnicoresponsavel,tb_feedback.datacadastro, tb_ticket.id AS idticket " +
                       $"FROM tb_feedback " +
                       $"INNER JOIN tb_ticket ON tb_feedback.fk_ticket = tb_ticket.id " +
                       $"WHERE tb_ticket.fk_usuario = '{feedback.Ticket.Usuario.Id}' AND tb_feedback.status ILIKE '%Novo%' " +
                       $"ORDER BY tb_feedback.datacadastro DESC";
                }
                DbDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows)
                    throw new Exception("Sem feedback registrado");

                while (dr.Read())
                {
                    Feedback fdb = new Feedback();
                    fdb.Id = Convert.ToInt32(dr["idfeedback"].ToString());
                    fdb.Status = dr["statusfeedback"].ToString();
                    fdb.NotaAtendimento = Convert.ToInt32(dr["notaatendimento"].ToString());
                    fdb.NotaAtendente = Convert.ToInt32(dr["notaatendente"].ToString());
                    fdb.Comentarios = dr["comentarios"].ToString();
                    fdb.Ticket.CodigoTicket = dr["codigoticket"].ToString();
                    fdb.Ticket.TecnicoResponsavel = dr["tecnicoresponsavel"].ToString();
                    fdb.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());
                    fdb.Ticket.Id = Convert.ToInt32(dr["idticket"].ToString());

                    feedbacks.Add(fdb);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return feedbacks.ToList<IEntidade>();
        }

        public override void Excluir(EntidadeDominio entidade)
        {
            base.Excluir(entidade);
        }

        public override void Inserir(EntidadeDominio entidade)
        {
            Ticket ticket = (Ticket)entidade;

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;


                //inicializa o feedback sem atribuir nota ou alguma informação para o usuário
                cmd.CommandText = $"INSERT INTO tb_feedback(datacadastro,status,notaatendimento,notaatendente,fk_ticket) VALUES('{ticket.DataResolucao}'," +
                $"'Novo',0,0,'{ticket.Id}')";

                cmd.ExecuteNonQuery();
                Commit();




            }
            catch (Exception ex)
            {
                Rollback();
                CloseConnection();

                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
