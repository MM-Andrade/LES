using Dominio;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class AnaliseTicketDAO : AbstractDAO
    {
        public AnaliseTicketDAO() : base("tb_ticket", "id")
        {

        }
        public AnaliseTicketDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public AnaliseTicketDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override List<IEntidade> Consultar(IEntidade entidade)
        {
            AnaliseTicket analise = (AnaliseTicket)entidade;
            List<AnaliseTicket> tickets = new List<AnaliseTicket>();

            try
            {

                OpenConnection();
                DbCommand cmd = GetDbCommand(dbConnection);

                if (analise.StrAcao == "Abertos")
                {
                    cmd.CommandText = $"SELECT tb_ticket.datacadastro,tb_tiposervico.nomeservico " +
                                    $"FROM tb_ticket " +
                                   $"INNER JOIN tb_tiposervico ON tb_ticket.fk_tiposervico = tb_tiposervico.id " +
                                   $"INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                                   $"WHERE tb_ticket.datacadastro BETWEEN '{analise.DataInicioAnalise}' AND '{analise.DataFinalAnalise}'" +
                                   $"AND tb_departamento.nomedepartamento ILIKE '%{analise.GetTicket.TipoServico.Departamento.NomeDepartamento}%' " +
                                   $"ORDER BY tb_ticket.datacadastro ASC";

                    DbDataReader dr = cmd.ExecuteReader();
                    tickets = DataReaderAnalise(dr);
                }
                else if (analise.StrAcao == "Violados")
                {
                    cmd.CommandText = $"SELECT tb_ticket.datacadastro,tb_tiposervico.nomeservico " +
                                    $"FROM tb_ticket " +
                                    $"INNER JOIN tb_tiposervico ON tb_ticket.fk_tiposervico = tb_tiposervico.id " +
                                    $"INNER JOIN tb_slaticket ON tb_slaticket.fk_ticket = tb_ticket.id " +
                                    $"INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                                    $"WHERE tb_ticket.datacadastro BETWEEN '{analise.DataInicioAnalise}' AND '{analise.DataFinalAnalise}'" +
                                    $"AND tb_departamento.nomedepartamento ILIKE '%{analise.GetTicket.TipoServico.Departamento.NomeDepartamento}%' " +
                                    $"AND tb_slaticket.status ILIKE 'VIOLADO' ";

                    DbDataReader dr = cmd.ExecuteReader();
                    tickets = DataReaderAnalise(dr);
                }
                else if (analise.StrAcao == "Resolvidos")
                {
                    cmd.CommandText = $"SELECT tb_ticket.datacadastro,tb_tiposervico.nomeservico " +
                                    $"FROM tb_ticket " +
                                  $"INNER JOIN tb_tiposervico ON tb_ticket.fk_tiposervico = tb_tiposervico.id " +
                                  $"INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                                  $"WHERE tb_ticket.datacadastro BETWEEN '{analise.DataInicioAnalise}' AND '{analise.DataFinalAnalise}'" +
                                  $"AND tb_departamento.nomedepartamento ILIKE '%{analise.GetTicket.TipoServico.Departamento.NomeDepartamento}%' " +
                                  $"AND tb_ticket.status ILIKE 'Fechado_Resolvido' " +
                                  $"OR  tb_ticket.status ILIKE 'Resolvido' ";

                    DbDataReader dr = cmd.ExecuteReader();
                    tickets = DataReaderAnalise(dr);
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

            return tickets.ToList<IEntidade>();
        }

        private List<AnaliseTicket> DataReaderAnalise(DbDataReader dr)
        {
            if (!dr.HasRows)
            {
                throw new Exception("Sem Registros");
            }

            List<AnaliseTicket> tickets = new List<AnaliseTicket>();
            while (dr.Read())
            {
                AnaliseTicket t = new AnaliseTicket();

                t.GetTicket.TipoServico.NomeServico = dr["nomeservico"].ToString();
                t.GetTicket.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());

                tickets.Add(t);
            }
            dr.Close();

            return tickets.ToList();
        }

        public List<AnaliseTicket> DataReaderGenerico(DbDataReader dr)
        {
            if (!dr.HasRows)
            {
                throw new Exception("Sem Registros");
            }

            List<AnaliseTicket> tickets = new List<AnaliseTicket>();
            while (dr.Read())
            {
                AnaliseTicket t = new AnaliseTicket();

                t.GetTicket.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());
                t.GetTicket.CodigoTicket = dr["codigoticket"].ToString();
                t.GetTicket.TecnicoResponsavel = dr["tecnicoresponsavel"].ToString();
                t.GetTicket.DataAtualizacao = DateTime.Parse(dr["dataatualizacao"].ToString());
                t.GetTicket.Status = dr["status"].ToString();
                t.GetTicket.DataResolucao = DateTime.Parse(dr["dataresolucao"].ToString());
                t.GetTicket.DataEncerramento = DateTime.Parse(dr["dataencerramento"].ToString());
                t.GetTicket.TipoServico.NomeServico = dr["nomeservico"].ToString();


                tickets.Add(t);
            }
            dr.Close();

            return tickets.ToList();
        }
    }
}
