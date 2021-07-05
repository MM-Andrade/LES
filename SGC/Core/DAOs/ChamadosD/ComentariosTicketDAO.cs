using Core.Aplicacao;
using Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class ComentariosTicketDAO : AbstractDAO
    {

        public ComentariosTicketDAO() : base("tb_comentariosticket", "id")
        {

        }
        public ComentariosTicketDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public ComentariosTicketDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
            base.Atualizar(entidade);
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            if (entidade == null)
            {
                return null;
            }

            ComentariosTicket ticket = (ComentariosTicket)entidade;
            resultado = new Resultado();

            List<ComentariosTicket> comentarios = new List<ComentariosTicket>();

            try
            {
                OpenConnection();

                DbCommand cmd = GetDbCommand(dbConnection);

                cmd.CommandType = CommandType.Text;
                if (string.IsNullOrEmpty(ticket.CodigoTicket))
                {
                    throw new Exception("Sem código do ticket");
                }
                else
                {
                    cmd.CommandText = $"SELECT tb_ticket.codigoticket,tb_usuario.nome,tb_usuario.sobrenome, tb_comentarioticket.datacadastro, tb_comentarioticket.comentario FROM tb_ticket " +
                    $"INNER JOIN tb_comentarioticket ON tb_comentarioticket.fk_ticket = tb_ticket.id " +
                    $"INNER JOIN public.tb_usuario ON public.tb_comentarioticket.fk_usuario = public.tb_usuario.id " +
                    $"WHERE tb_ticket.codigoticket LIKE '{ticket.CodigoTicket}' ORDER BY tb_comentarioticket.datacadastro DESC";
                }
                DbDataReader dr = cmd.ExecuteReader();
                comentarios = DataReaderComentario(dr);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return comentarios.ToList<IEntidade>();
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
                if (ticket.StrAcao == "JobEncerrarTicket")
                {
                    cmd.CommandText = $"INSERT INTO tb_comentarioticket" +
                        $"(datacadastro,comentario,fk_ticket) " +
                        $"VALUES ('{ticket.DataEncerramento}','{ticket.ComentariosTicket.Comentario}'," +
                        $"'{ticket.Id}') RETURNING id";

                    ticket.ComentariosTicket.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    Commit();
                }
                else
                {
                    cmd.CommandText = $"INSERT INTO tb_comentarioticket(datacadastro,comentario,fk_usuario,fk_ticket) VALUES (" +
                        $"'{ticket.DataAtualizacao}','{ticket.ComentariosTicket.Comentario}'," +
                        $"'{ticket.Usuario.Id}','{ticket.Id}') RETURNING id";

                    ticket.ComentariosTicket.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    Commit();
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

            resultado.Entidades.Add(ticket);
        }


        private List<ComentariosTicket> DataReaderComentario(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Sem comentários...");

            List<ComentariosTicket> comentarios = new List<ComentariosTicket>();

            while (dr.Read())
            {
                ComentariosTicket t = new ComentariosTicket();

                t.CodigoTicket = dr["codigoticket"].ToString();
                t.Usuario.Nome = dr["nome"].ToString();
                t.Usuario.Sobrenome = dr["sobrenome"].ToString();
                t.Comentario = dr["comentario"].ToString();
                t.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());

                comentarios.Add(t);
            }
            dr.Close();

            return comentarios;
        }
    }
}
