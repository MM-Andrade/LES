using Core.Aplicacao;
using Core.Controle;
using Dominio;
using Dominio.Chamados;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Core.DAOs
{
    public class TicketDAO : AbstractDAO
    {
        public TicketDAO() : base("tb_ticket", "id")
        {

        }
        public TicketDAO(string table, string idTable) : base(table, idTable)
        {
        }

        public TicketDAO(DbConnection dbConnection, string table, string idTable) : base(dbConnection, table, idTable)
        {
        }

        public override void Atualizar(EntidadeDominio entidade)
        {

            Ticket ticket = (Ticket)entidade;
            resultado = new Resultado();
            

            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;


                if (ticket.StrAcao == "JobEncerrarTicket")
                {

                    cmd.CommandText = $"UPDATE tb_ticket SET status = '{ticket.Status}', dataencerramento = '{ticket.DataEncerramento}' " +
                        $"WHERE '{ticket.DataEncerramento}' >=  tb_ticket.dataresolucao + INTERVAL '3' DAY AND tb_ticket.status ILIKE '%Resolvido%'";

                    //cmd.CommandText = $"UPDATE tb_ticket SET status = 'FECHADO', dataencerramento = CURRENT_TIMESTAMP " +
                    //    $" WHERE tb_ticket.dataresolucao + INTERVAL '3' DAY >= CURRENT_TIMESTAMP AND tb_ticket.status ILIKE '%Resolvido%'";


                    cmd.ExecuteNonQuery();
                    Commit();


                }
                else if (ticket.StrAcao == "ResolverChamado")
                {

                    Estoque estoque = new Estoque();
                    estoque.GetTicket = ticket;
                    ServicosUtilizados servicosUtilizados = new ServicosUtilizados();
                    servicosUtilizados.GetTicket = ticket;

                    cmd.CommandText = $"UPDATE tb_ticket SET status = '{ticket.Status}', tecnicoresponsavel = '{ticket.TecnicoResponsavel}'," +
                                     $"dataatualizacao = '{ticket.DataAtualizacao}',descricao = '{ticket.Descricao}', dataresolucao='{ticket.DataResolucao}'" +
                                     $"WHERE tb_ticket.codigoticket LIKE '{ticket.CodigoTicket}' RETURNING id";

                    ticket.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    Commit();


                    HistoricoTicketDAO historicoTicketDAO = new HistoricoTicketDAO();
                    ComentariosTicketDAO comentariosTicketDAO = new ComentariosTicketDAO();
                    FeedbackDAO feedbackDAO = new FeedbackDAO();




                    foreach (int item in ticket.ItensEstoque)
                    {
                        estoque.Id = item;
                        resultado = new Fachada().Atualizar(estoque);
                    }

                    foreach (int item in ticket.ItensServico)
                    {
                        servicosUtilizados.GetItemServico.Id = item;
                        resultado = new Fachada().Inserir(servicosUtilizados);
                    }

                    comentariosTicketDAO.Inserir(ticket);
                    historicoTicketDAO.Inserir(ticket);
                    feedbackDAO.Inserir(ticket);
                }

                else
                {
                    cmd.CommandText = $"UPDATE tb_ticket SET status = '{ticket.Status}', tecnicoresponsavel = '{ticket.TecnicoResponsavel}'," +
                                      $"dataatualizacao = '{ticket.DataAtualizacao}',descricao = '{ticket.Descricao}'" +
                                      $"WHERE tb_ticket.codigoticket LIKE '{ticket.CodigoTicket}' RETURNING id";

                    ticket.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    Commit();

                    HistoricoTicketDAO historicoTicketDAO = new HistoricoTicketDAO();
                    ComentariosTicketDAO comentariosTicketDAO = new ComentariosTicketDAO();




                    comentariosTicketDAO.Inserir(ticket);
                    historicoTicketDAO.Inserir(ticket);
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

        public override List<IEntidade> Consultar(IEntidade entidade)
        {

            Ticket ticket = (Ticket)entidade; //nunca instanciar um novo objeto, senão tu zera ele...
            List<Ticket> tickets = new List<Ticket>();

            try
            {

                OpenConnection();
                DbCommand cmd = GetDbCommand(dbConnection);
                if (string.IsNullOrEmpty(ticket.StrBusca))//busca por um inc?
                {//não...
                    if (ticket.StrAcao == "ConsultarSLA")
                    {//é uma consulta de SLA para calculo? - Sim
                        cmd.CommandText = $"SELECT tb_ticket.codigoticket,tb_ticket.datacadastro,tb_ticket.datacadastro + INTERVAL '{ticket.TipoServico.Prioridade.TempoAtendimento}' HOUR as encerramentoprevisto," +
                            $"tb_tiposervico.nomeservico,tb_prioridade.tempodeatendimento " +
                            $"FROM tb_ticket " +
                            $"INNER JOIN tb_tiposervico ON tb_ticket.fk_tiposervico = tb_tiposervico.id " +
                            $"INNER JOIN tb_prioridade ON tb_tiposervico.fk_prioridade = tb_prioridade.id " +
                            $"WHERE tb_ticket.codigoticket LIKE '{ticket.CodigoTicket}'";

                        DbDataReader dr = cmd.ExecuteReader();
                        tickets = DataReaderSla(dr);
                    }
                    else if (ticket.StrAcao == "JobEncerrarTicket")
                    {//é o job encerrador verificando os tickets? sim...
                        cmd.CommandText = $"SELECT " +
                            $"tb_ticket.codigoticket,tb_ticket.dataresolucao,tb_ticket.status," +
                            $"tb_ticket.dataresolucao + INTERVAL '3' DAY AS dataencerramento " +
                            $"FROM tb_ticket " +
                            $"WHERE tb_ticket.status NOT ILIKE '%Cancelado%' " +
                            $"AND tb_ticket.status NOT LIKE '%Fechado%'";

                        DbDataReader dr = cmd.ExecuteReader();
                        tickets = DataReaderJobFechamento(dr);
                    }
                    else if (ticket.StrAcao == "MinhaFila")
                    {
                        cmd.CommandText = $"SELECT tb_ticket.id AS idticket,tb_ticket.datacadastro,tb_ticket.codigoticket,tb_ticket.tecnicoresponsavel,tb_ticket.descricao,tb_ticket.dataatualizacao,tb_ticket.status,tb_ticket.dataencerramento,tb_ticket.dataresolucao,tb_ticket.tempototalatendimento,tb_ticket.descricao,tb_tiposervico.nomeservico,tb_tiposervico.status,tb_tiposervico.datacadastro,tb_departamento.nomedepartamento,tb_departamento.id AS iddepartamento, tb_prioridade.tempodeatendimento,tb_prioridade.descricaoprioridade,tb_prioridade.nivelprioridade,tb_usuario.sobrenome,tb_usuario.nome,tb_usuario.email,tb_usuario.id AS idusuario, tb_slaticket.status as statusla " +
                            $"FROM tb_ticket " +
                            $"INNER JOIN tb_tiposervico ON tb_ticket.fk_tiposervico = tb_tiposervico.id " +
                            $"INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                            $"INNER JOIN tb_prioridade ON tb_tiposervico.fk_prioridade = tb_prioridade.id " +
                            $"INNER JOIN tb_slaticket ON tb_slaticket.fk_ticket = tb_ticket.id " +
                            $"INNER JOIN tb_usuario ON tb_ticket.fk_usuario = tb_usuario.id " +
                            $"WHERE tb_departamento.id = '{ticket.Usuario.GrupoAtendimento.Departamento.Id}'  ORDER BY tb_ticket.datacadastro DESC";

                        DbDataReader dr = cmd.ExecuteReader();
                        tickets = DataReaderMinhaFila(dr);
                    }
                    else if (ticket.Usuario.Id > 0 && ticket.Usuario.Funcao.Id != 3 && ticket.StrAcao == "BuscaAdmin") //é uma pesquisa geral feita por um técnico/adiministrador? 
                    {//sim
                        cmd.CommandText = $"SELECT tb_ticket.id AS idticket,tb_ticket.datacadastro,tb_ticket.codigoticket,tb_ticket.tecnicoresponsavel,tb_ticket.descricao,tb_ticket.dataatualizacao,tb_ticket.status,tb_ticket.dataencerramento,tb_ticket.dataresolucao,tb_ticket.tempototalatendimento,tb_ticket.descricao,tb_tiposervico.nomeservico,tb_tiposervico.status,tb_tiposervico.datacadastro,tb_departamento.nomedepartamento,tb_departamento.id AS iddepartamento, tb_prioridade.tempodeatendimento,tb_prioridade.descricaoprioridade,tb_prioridade.nivelprioridade,tb_usuario.sobrenome,tb_usuario.nome,tb_usuario.email,tb_usuario.id AS idusuario,tb_slaticket.status as statusla  " +
                            $"FROM tb_ticket " +
                            $"INNER JOIN tb_tiposervico ON tb_ticket.fk_tiposervico = tb_tiposervico.id " +
                            $"INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                            $"INNER JOIN tb_prioridade ON tb_tiposervico.fk_prioridade = tb_prioridade.id " +
                            $"INNER JOIN tb_usuario ON tb_ticket.fk_usuario = tb_usuario.id " +
                            $"INNER JOIN tb_slaticket ON tb_slaticket.fk_ticket = tb_ticket.id " +
                            $"ORDER BY tb_ticket.datacadastro DESC";

                        DbDataReader dr = cmd.ExecuteReader();
                        tickets = DataReaderMinhaFila(dr);
                    }
                    else if (!string.IsNullOrEmpty(ticket.CodigoTicket))
                    {
                        cmd.CommandText = $"SELECT tb_ticket.id AS idticket,tb_ticket.datacadastro,tb_ticket.codigoticket,tb_ticket.tecnicoresponsavel,tb_ticket.descricao,tb_ticket.dataatualizacao,tb_ticket.status,tb_ticket.dataencerramento,tb_ticket.dataresolucao,tb_ticket.tempototalatendimento,tb_ticket.descricao,tb_tiposervico.nomeservico,tb_tiposervico.status,tb_tiposervico.datacadastro,tb_departamento.nomedepartamento,tb_departamento.id AS iddepartamento, tb_prioridade.tempodeatendimento,tb_prioridade.descricaoprioridade,tb_prioridade.nivelprioridade,tb_usuario.sobrenome,tb_usuario.nome,tb_usuario.email,tb_usuario.id AS idusuario " +
                            $"FROM tb_ticket " +
                            $"INNER JOIN tb_tiposervico ON tb_ticket.fk_tiposervico = tb_tiposervico.id " +
                            $"INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                            $"INNER JOIN tb_prioridade ON tb_tiposervico.fk_prioridade = tb_prioridade.id " +
                            $"INNER JOIN tb_usuario ON tb_ticket.fk_usuario = tb_usuario.id " +
                            $"WHERE tb_ticket.codigoticket LIKE '{ticket.CodigoTicket}'  ORDER BY tb_ticket.datacadastro DESC";

                        DbDataReader dr = cmd.ExecuteReader();
                        tickets = DataReaderGenerico(dr);
                    }
                    else if (ticket.Usuario.Id > 0) //é um usuário pesquisando seus tickets?
                    {//sim
                        cmd.CommandText = $"SELECT tb_ticket.id AS idticket,tb_ticket.datacadastro,tb_ticket.codigoticket,tb_ticket.tecnicoresponsavel,tb_ticket.descricao,tb_ticket.dataatualizacao,tb_ticket.status,tb_ticket.dataencerramento,tb_ticket.dataresolucao,tb_ticket.tempototalatendimento,tb_ticket.descricao,tb_tiposervico.nomeservico,tb_tiposervico.status,tb_tiposervico.datacadastro,tb_departamento.nomedepartamento,tb_departamento.id AS iddepartamento, tb_prioridade.tempodeatendimento,tb_prioridade.descricaoprioridade,tb_prioridade.nivelprioridade,tb_usuario.sobrenome,tb_usuario.nome,tb_usuario.email,tb_usuario.id AS idusuario " +
                            $"FROM tb_ticket " +
                            $"INNER JOIN tb_tiposervico ON tb_ticket.fk_tiposervico = tb_tiposervico.id " +
                            $"INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                            $"INNER JOIN tb_prioridade ON tb_tiposervico.fk_prioridade = tb_prioridade.id " +
                            $"INNER JOIN tb_usuario ON tb_ticket.fk_usuario = tb_usuario.id " +
                           $"WHERE tb_ticket.fk_usuario = '{ticket.Usuario.Id}' ORDER BY tb_ticket.datacadastro DESC";

                        DbDataReader dr = cmd.ExecuteReader();
                        tickets = DataReaderGenerico(dr);
                    }
                }
                else if (ticket.Usuario.Id > 0) //é um usuário pesquisando seus tickets?
                {//sim
                    cmd.CommandText = $"SELECT tb_ticket.id AS idticket,tb_ticket.datacadastro,tb_ticket.codigoticket,tb_ticket.tecnicoresponsavel,tb_ticket.descricao,tb_ticket.dataatualizacao,tb_ticket.status,tb_ticket.dataencerramento,tb_ticket.dataresolucao,tb_ticket.tempototalatendimento,tb_ticket.descricao,tb_tiposervico.nomeservico,tb_tiposervico.status,tb_tiposervico.datacadastro,tb_departamento.nomedepartamento,tb_departamento.id AS iddepartamento, tb_prioridade.tempodeatendimento,tb_prioridade.descricaoprioridade,tb_prioridade.nivelprioridade,tb_usuario.sobrenome,tb_usuario.nome,tb_usuario.email,tb_usuario.id AS idusuario " +
                        $"FROM tb_ticket " +
                        $"INNER JOIN tb_tiposervico ON tb_ticket.fk_tiposervico = tb_tiposervico.id " +
                        $"INNER JOIN tb_departamento ON tb_tiposervico.fk_departamento = tb_departamento.id " +
                        $"INNER JOIN tb_prioridade ON tb_tiposervico.fk_prioridade = tb_prioridade.id " +
                        $"INNER JOIN tb_usuario ON tb_ticket.fk_usuario = tb_usuario.id " +
                       $"WHERE tb_ticket.fk_usuario = '{ticket.Usuario.Id}' ORDER BY tb_ticket.datacadastro DESC";

                    DbDataReader dr = cmd.ExecuteReader();
                    tickets = DataReaderGenerico(dr);
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

        public override void Excluir(EntidadeDominio entidade)
        {
            base.Excluir(entidade);
        }

        public override void Inserir(EntidadeDominio entidade)
        {

            Ticket ticket = (Ticket)entidade;
            resultado = new Resultado();


            HistoricoTicketDAO historicoTicketDAO = new HistoricoTicketDAO();
            SlaTicket slaTicket = new SlaTicket();


            try
            {
                OpenConnection();
                BeginTransaction();

                DbCommand cmd = GetDbCommand(dbConnection);
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = $"INSERT INTO tb_ticket (datacadastro,codigoticket,tecnicoresponsavel,dataatualizacao,status,dataresolucao,dataencerramento,tempototalatendimento," +
                    $"descricao,fk_usuario,fk_tiposervico)" +
                    $" VALUES ('{ticket.DataCadastro}','{ticket.CodigoTicket}','{ticket.TecnicoResponsavel}','{ticket.DataAtualizacao}','{ticket.Status}'," +
                    $"'{ticket.DataResolucao}','{ticket.DataEncerramento}','{ticket.TempoTotalAtendimento}','{ticket.Descricao}'," +
                    $"'{ticket.Usuario.Id}','{ticket.TipoServico.Id}') RETURNING id";

                ticket.Id = Convert.ToInt32(cmd.ExecuteScalar());
                Commit();

                slaTicket.Ticket = ticket;

                //inicia SLA
                resultado = new Fachada().Inserir(slaTicket);
                //inicia histórico de chamado..
                historicoTicketDAO.Inserir(ticket);

            }
            catch (NpgsqlException nex)
            {
                Rollback();
                throw nex;
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


        public List<Ticket> DataReaderNovoRegistro(DbDataReader dr)
        {
            if (!dr.HasRows)
                throw new Exception("Sem registros");

            List<Ticket> tickets = new List<Ticket>();
            while (dr.Read())
            {
                Ticket t = new Ticket();
                t.CodigoTicket = dr["codigoticket"].ToString();


                tickets.Add(t);
            }
            dr.Close();

            return tickets;
        }
        public List<Ticket> DataReaderGenerico(DbDataReader dr)
        {
            if (!dr.HasRows)
            {
                throw new Exception("Sem Registros");
            }

            List<Ticket> tickets = new List<Ticket>();
            while (dr.Read())
            {
                Ticket t = new Ticket();
                t.Id = Convert.ToInt32(dr["idticket"].ToString());
                t.Usuario.Id = Convert.ToInt32(dr["idusuario"].ToString());
                t.TipoServico.Departamento.Id = Convert.ToInt32(dr["iddepartamento"].ToString());

                t.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());
                t.CodigoTicket = dr["codigoticket"].ToString();
                t.TecnicoResponsavel = dr["tecnicoresponsavel"].ToString();
                t.DataAtualizacao = DateTime.Parse(dr["dataatualizacao"].ToString());
                t.Status = dr["status"].ToString();
                t.Descricao = dr["descricao"].ToString();
                t.Usuario.Nome = dr["nome"].ToString();
                t.Usuario.Sobrenome = dr["sobrenome"].ToString();
                t.Usuario.Email = dr["email"].ToString();

                t.TipoServico.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();
                t.TipoServico.Prioridade.TempoAtendimento = Convert.ToInt32(dr["tempodeatendimento"].ToString());
                t.TipoServico.Prioridade.DescricaoPrioridade = dr["descricaoprioridade"].ToString();
                t.TipoServico.Prioridade.NivelPrioridade = Convert.ToInt32(dr["nivelprioridade"].ToString());
                t.TipoServico.NomeServico = dr["nomeservico"].ToString();
                t.TipoServico.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();



                //t.TipoServico.Categoria.NomeCategoria = dr["nomecategoria"].ToString();
                tickets.Add(t);
            }
            dr.Close();

            return tickets.ToList();
        }
        public List<Ticket> DataReaderSla(DbDataReader dr)
        {
            if (!dr.HasRows)
            {
                throw new Exception("Sem Registros");
            }

            List<Ticket> tickets = new List<Ticket>();
            while (dr.Read())
            {
                Ticket t = new Ticket();
                t.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());
                t.CodigoTicket = dr["codigoticket"].ToString();
                t.TipoServico.NomeServico = dr["nomeservico"].ToString();
                t.TipoServico.Prioridade.TempoAtendimento = Convert.ToInt32(dr["tempodeatendimento"].ToString());
                t.SlaTicket.DataPrevista = DateTime.Parse(dr["encerramentoprevisto"].ToString());

                tickets.Add(t);
            }
            dr.Close();

            return tickets.ToList();
        }
        private List<Ticket> DataReaderJobFechamento(DbDataReader dr)
        {
            if (!dr.HasRows)
            {
                throw new Exception("Sem Registros");
            }

            List<Ticket> tickets = new List<Ticket>();
            while (dr.Read())
            {
                Ticket t = new Ticket();
                t.DataCadastro = DateTime.Parse(dr["dataresolucao"].ToString());
                t.CodigoTicket = dr["codigoticket"].ToString();
                t.Status = dr["status"].ToString();
                t.DataEncerramento = DateTime.Parse(dr["dataencerramento"].ToString());

                tickets.Add(t);
            }
            dr.Close();

            return tickets.ToList();
        }

        public List<Ticket> DataReaderMinhaFila(DbDataReader dr)
        {
            if (!dr.HasRows)
            {
                throw new Exception("Sem Registros");
            }

            List<Ticket> tickets = new List<Ticket>();
            while (dr.Read())
            {
                Ticket t = new Ticket();
                t.Id = Convert.ToInt32(dr["idticket"].ToString());
                t.Usuario.Id = Convert.ToInt32(dr["idusuario"].ToString());
                t.TipoServico.Departamento.Id = Convert.ToInt32(dr["iddepartamento"].ToString());

                t.DataCadastro = DateTime.Parse(dr["datacadastro"].ToString());
                t.CodigoTicket = dr["codigoticket"].ToString();
                t.TecnicoResponsavel = dr["tecnicoresponsavel"].ToString();
                t.DataAtualizacao = DateTime.Parse(dr["dataatualizacao"].ToString());
                t.Status = dr["status"].ToString();
                t.Descricao = dr["descricao"].ToString();
                t.Usuario.Nome = dr["nome"].ToString();
                t.Usuario.Sobrenome = dr["sobrenome"].ToString();
                t.Usuario.Email = dr["email"].ToString();

                t.TipoServico.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();
                t.TipoServico.Prioridade.TempoAtendimento = Convert.ToInt32(dr["tempodeatendimento"].ToString());
                t.TipoServico.Prioridade.DescricaoPrioridade = dr["descricaoprioridade"].ToString();
                t.TipoServico.Prioridade.NivelPrioridade = Convert.ToInt32(dr["nivelprioridade"].ToString());
                t.TipoServico.NomeServico = dr["nomeservico"].ToString();
                t.TipoServico.Departamento.NomeDepartamento = dr["nomedepartamento"].ToString();

                t.SlaTicket.Status = dr["statusla"].ToString();

                //t.TipoServico.Categoria.NomeCategoria = dr["nomecategoria"].ToString();
                tickets.Add(t);
            }
            dr.Close();

            return tickets.ToList();
        }
    }
}
