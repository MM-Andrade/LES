using Core.Aplicacao;
using Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class SlaTicketDAO : AbstractDAO
    {
        public SlaTicketDAO() : base("tb_slaticket", "id")
        {

        }

        public SlaTicketDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public SlaTicketDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {
            SlaTicket slaTicket = (SlaTicket)entidade;
            Ticket ticket = slaTicket.Ticket;

            resultado.Entidades.Add(ticket);

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                if (slaTicket.StrAcao == "JobVerificaSlaViolado")
                {

                    /* FAZ UPDATE PARA VIOLADO DE TODOS OS REGISTROS QUE:
                     * Estão acima da data prevista E
                     * o STATUS não é VIOLADO E
                     * a DATA de Resolução é maior que a data prevista
                    */
                    cmd.CommandText = $"UPDATE tb_slaticket SET " +
                        $"status = 'VIOLADO', " +
                        $"dataviolacao = '{DateTime.Now}', " +
                        $"motivoviolacao = 'Violado por não atendimento dentro do período' " +
                        $"FROM tb_ticket " +
                        $"WHERE  tb_slaticket.dataprevista <= tb_ticket.dataresolucao " +
                        $"AND tb_slaticket.status NOT ILIKE '%VIOLADO%' " +
                        $"AND tb_ticket.status NOT ILIKE 'CANCELADO' " +
                        $"AND tb_ticket.status NOT ILIKE 'Fechado Resolvido' " +
                        $"AND tb_ticket.status NOT ILIKE 'Pendente' " +
                        $"OR  tb_slaticket.dataprevista <= tb_ticket.dataresolucao " +
                        $"AND tb_slaticket.status NOT ILIKE '%VIOLADO%' " +
                        $"AND tb_ticket.status NOT ILIKE 'CANCELADO' " +
                        $"AND tb_ticket.status NOT ILIKE 'Fechado Resolvido' " +
                        $"AND tb_ticket.status NOT ILIKE 'Pendente' ";
                }

                cmd.ExecuteNonQuery();
                Commit();
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

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            SlaTicket slaTicket = (SlaTicket)entidade;
            Ticket ticket = slaTicket.Ticket;

            List<Ticket> tickets = new List<Ticket>();
            try
            {
                OpenConnection();

                DbCommand cmd = GetDbCommand(dbConnection);
                if (slaTicket.StrAcao == "JobVerificaSLA")
                {
                    cmd.CommandText = $"SELECT tb_slaticket.dataprevista,tb_slaticket.status,tb_slaticket.percentilatendimento,tb_slaticket.dataviolacao,tb_slaticket.motivoviolacao,tb_ticket.codigoticket,tb_ticket.datacadastro" +
                        $"FROM tb_slaticket " +
                        $"INNER JOIN tb_ticket ON tb_slaticket.fk_ticket = tb_ticket.id " +
                        $"WHERE tb_slaticket.dataprevista < '{DateTime.Now}'";
                }
                else if (slaTicket.StrAcao == "SLAs" && string.IsNullOrEmpty(slaTicket.StrBusca))
                {//pesquisa geral de SLA dessa P¨&*@¨*#!!

                    cmd.CommandText = $"SELECT tb_slaticket.status,tb_slaticket.percentilatendimento,tb_slaticket.dataviolacao,tb_slaticket.dataprevista,tb_ticket.codigoticket,tb_ticket.datacadastro,tb_slaticket.motivoviolacao " +
                   $"FROM tb_slaticket " +
                   $"INNER JOIN tb_ticket ON tb_slaticket.fk_ticket = tb_ticket.id ORDER BY tb_ticket.datacadastro DESC ";
                }
                else if (slaTicket.StrAcao == "SLAs" && !string.IsNullOrEmpty(slaTicket.StrBusca))
                {//busca a partir de um termo digitado na p. do formulário de sla...
                    cmd.CommandText = $"SELECT tb_slaticket.status,tb_slaticket.percentilatendimento,tb_slaticket.dataviolacao,tb_slaticket.dataprevista,tb_ticket.codigoticket,tb_ticket.datacadastro,tb_slaticket.motivoviolacao " +
                        $"FROM tb_slaticket " +
                        $"INNER JOIN tb_ticket ON tb_slaticket.fk_ticket = tb_ticket.id " +
                        $"WHERE tb_ticket.codigoticket ILIKE '{slaTicket.StrBusca}'" +
                        $"OR tb_slaticket.status ILIKE '%{slaTicket.StrBusca}%' ";
                }
                else if (!string.IsNullOrEmpty(ticket.CodigoTicket))
                {
                    cmd.CommandText = $"SELECT tb_slaticket.status,tb_slaticket.percentilatendimento,tb_slaticket.dataviolacao,tb_slaticket.dataprevista,tb_ticket.codigoticket,tb_ticket.datacadastro,tb_slaticket.motivoviolacao " +
                        $"FROM tb_slaticket " +
                        $"INNER JOIN tb_ticket ON tb_slaticket.fk_ticket = tb_ticket.id " +
                        $"WHERE tb_ticket.codigoticket LIKE '{ticket.CodigoTicket}'";
                }


                DbDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows)
                    throw new Exception("Sem SLA registrado");

                while (dr.Read())
                {
                    Ticket t = new Ticket();
                    t.SlaTicket.Status = dr["status"].ToString();
                    t.SlaTicket.PercentilAtendimento = dr["percentilatendimento"].ToString();
                    t.SlaTicket.DataViolacao = DateTime.Parse(dr["dataviolacao"].ToString());
                    t.SlaTicket.DataPrevista = DateTime.Parse(dr["dataprevista"].ToString());
                    t.SlaTicket.Motivo = dr["motivoviolacao"].ToString();
                    t.CodigoTicket = dr["codigoticket"].ToString();
                    t.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());

                    tickets.Add(t);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tickets.ToList<IEntidade>();
        }

        public override void Excluir(EntidadeDominio entidade)
        {
            base.Excluir(entidade);
        }

        public override void Inserir(EntidadeDominio entidade)
        {

            SlaTicket slaTicket = (SlaTicket)entidade;
            Ticket ticket = slaTicket.Ticket;

            resultado.Entidades.Add(ticket);

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = $"INSERT INTO tb_slaticket(status,dataviolacao,dataprevista,fk_ticket) VALUES(" +
                    $"'{ticket.SlaTicket.Status}','{ticket.SlaTicket.DataViolacao}','{ticket.SlaTicket.DataPrevista}','{ticket.Id}')";

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
