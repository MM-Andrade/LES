using Core.Aplicacao;
using Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class HistoricoTicketDAO : AbstractDAO
    {
        public HistoricoTicketDAO() : base("tb_historicoticket","id")
        {

        }
        public HistoricoTicketDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public HistoricoTicketDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
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

            HistoricoTicket ticket = (HistoricoTicket)entidade;
            resultado = new Resultado();

            List<HistoricoTicket> historicoTickets = new List<HistoricoTicket>();

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
                    cmd.CommandText = $"SELECT tb_historicoticket.dataatualizacao,tb_historicoticket.status,tb_historicoticket.statusanterior," +
                        $"tb_historicoticket.filadeatendimento,tb_historicoticket.tecnicoresponsavel," +
                        $"tb_historicoticket.prioridade,tb_historicoticket.descricao,tb_ticket.codigoticket,tb_comentarioticket.comentario " +
                        $"FROM tb_historicoticket " +
                        $"INNER JOIN tb_ticket ON tb_historicoticket.fk_ticket = tb_ticket.id " +
                        $"INNER JOIN tb_comentarioticket ON tb_comentarioticket.fk_ticket = tb_ticket.id AND tb_historicoticket.fk_comentarioticket = tb_comentarioticket.id " +
                        $"WHERE tb_ticket.codigoticket LIKE '{ticket.CodigoTicket}' ORDER BY tb_historicoticket.dataatualizacao DESC";
                }
                DbDataReader dr = cmd.ExecuteReader();
                historicoTickets = DatareaderHistorico(dr);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return historicoTickets.ToList<IEntidade>();
        }

        public override void Excluir(EntidadeDominio entidade)
        {
            base.Excluir(entidade);
        }

        public override void Inserir(EntidadeDominio entidade)
        {
            Ticket ticket = (Ticket)entidade;
            //HistoricoTicket historicoTicket = new HistoricoTicket();

            resultado = new Resultado();
            //resultado.Entidades.Add(ticket);          //não é necessário...


            try
            {
                if(dbConnection == null)
                {
                    OpenConnection();
                    BeginTransaction();
                }
               
                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                if(ticket.Status == "Novo")
                {
                    cmd.CommandText = $"INSERT INTO tb_historicoticket (dataatualizacao,status,statusanterior,filadeatendimento,tecnicoresponsavel,prioridade,descricao,fk_ticket)" +
                        $"VALUES ('{ticket.DataAtualizacao}','{ticket.Status}','{ticket.Status}','{ticket.TipoServico.Departamento.NomeDepartamento}','{ticket.TecnicoResponsavel}'," +
                        $"'{ticket.TipoServico.Prioridade.Id}','{ticket.Descricao}','{ticket.Id}')";

                    cmd.ExecuteNonQuery();
                    Commit();
                }
                else if(ticket.Status != "Novo")
                {
                    cmd.CommandText = $"INSERT INTO tb_historicoticket (dataatualizacao,status,statusanterior,tecnicoresponsavel,filadeatendimento,prioridade,fk_comentarioticket,fk_ticket)" +
                           $"VALUES ('{ticket.DataAtualizacao}','{ticket.Status}','{ticket.HistoricoTicket.StatusAnterior}','{ticket.TecnicoResponsavel}'," +
                           $"'{ticket.Usuario.GrupoAtendimento.NomeGrupo}','{ticket.TipoServico.Prioridade.DescricaoPrioridade}'," +
                           $"'{ticket.ComentariosTicket.Id}','{ticket.Id}')";

                    cmd.ExecuteNonQuery();
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
        }

        private List<HistoricoTicket> DatareaderHistorico(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Sem histórico...");

            List<HistoricoTicket> historicoTickets = new List<HistoricoTicket>();

            while (dr.Read())
            {
                HistoricoTicket historico = new HistoricoTicket();

                historico.CodigoTicket = dr["codigoticket"].ToString();
                historico.FilaDeAtendimento = dr["filadeatendimento"].ToString();
                historico.TecnicoResponsavel = dr["tecnicoresponsavel"].ToString();
                historico.Status = dr["status"].ToString();
                historico.Prioridade = dr["prioridade"].ToString();
                historico.StatusAnterior = dr["statusanterior"].ToString();
                historico.ComentariosTicket.Comentario = dr["comentario"].ToString();
                historico.Descricao = dr["descricao"].ToString();
                historico.DataAtualizacao = DateTime.Parse(dr["dataatualizacao"].ToString());

                historicoTickets.Add(historico);
            }
            dr.Close();

            return historicoTickets;
        }
    }

}
