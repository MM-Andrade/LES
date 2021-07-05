using Core.Aplicacao;
using Core.Controle;
using Core.Core;
using Dominio;
using System;
using System.Linq;

namespace Core.Negocio.Chamados
{
    public class StAtualizaTicket : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            Ticket ticket = (Ticket)entidade;
            if (ticket.StrAcao == "JobEncerrarTicket")
            {
                //para não dar erro no job, mantenha null
                return null;

            }
            else
            {
                if (entidade != null)
                {
                    Resultado resultado = new Resultado();

                    resultado = new Fachada().Consultar(ticket);

                    //atribui o valor de ticket para uma variavel temporaria ticket...
                    var varTicket = (Ticket)resultado.Entidades.FirstOrDefault();

                    do
                    {
                        if (ticket.StrAcao == "Selecione" || string.IsNullOrEmpty(ticket.StrAcao))
                        {
                            return "Selecione uma opção";
                        }
                        else if (ticket.StrAcao == "Atualizar" && ticket.Usuario.Funcao.Id != 3)
                        {
                            ticket.DataAtualizacao = DateTime.Now;
                            ticket.HistoricoTicket.StatusAnterior = ticket.Status;
                            ticket.TecnicoResponsavel = ticket.Usuario.Nome + " " + ticket.Usuario.Sobrenome; //recebe o valor do novo técnico para atualizar...
                            if (ticket.Status == "Em resolução" && varTicket.TecnicoResponsavel == ticket.TecnicoResponsavel)
                            {// o status e o tecnico responsáveis são os mesmos (Não tiveram alterações)
                                if (string.IsNullOrEmpty(ticket.ComentariosTicket.Comentario))
                                {
                                    return "Insira um comentário";
                                }
                                //regra Ok, retorna para continuar com a atualização
                            }
                            if (ticket.Status == "Resolvido" && ticket.Usuario.Funcao.Id != 3)
                            {// o status é resolvido (vai reativar) pq fechou incorretamente ou sei lá, fez cagada...
                                ticket.Status = "Atríbuído";
                                ticket.TecnicoResponsavel = "";
                                if (string.IsNullOrEmpty(ticket.ComentariosTicket.Comentario))
                                {
                                    return "Insira um comentário";
                                }
                                //regra Ok, retorna para continuar com a atualização
                            }
                            else if (varTicket.TecnicoResponsavel != ticket.TecnicoResponsavel)
                            {//o status é diferente e o tecnico é diferente...

                                ticket.Status = "Em resolução";
                                ticket.TecnicoResponsavel = ticket.Usuario.Nome + " " + ticket.Usuario.Sobrenome;

                                if (string.IsNullOrEmpty(ticket.ComentariosTicket.Comentario))
                                {
                                    return "Insira um comentário";
                                }
                            }

                        }
                        else if (ticket.StrAcao == "Atualizar")
                        {
                            if (varTicket.Usuario.Id == ticket.Usuario.Id || ticket.Usuario.Funcao.Id != 3) //quem quer atualizar é proprietario ou admin/técnico ? 
                            {//sim o proprietario do ticket quer atualizar.
                                ticket.DataAtualizacao = DateTime.Now;
                                if (string.IsNullOrEmpty(ticket.ComentariosTicket.Comentario))
                                {
                                    return "Insira um comentário";
                                }
                            }
                            else
                            {
                                return "Você não tem autorização para atualizar esse ticket";
                            }
                        }
                        else if (ticket.StrAcao == "Reativar")
                        {
                            if (varTicket.Usuario.Id == ticket.Usuario.Id || ticket.Usuario.Funcao.Id != 3) //quem quer atualizar é proprietario ou admin/técnico ? 
                            {//sim o proprietario do ticket quer atualizar.
                                ticket.HistoricoTicket.StatusAnterior = ticket.Status;
                                ticket.Status = "Atribuido";
                                ticket.TecnicoResponsavel = "";
                                ticket.DataAtualizacao = DateTime.Now;
                                if (string.IsNullOrEmpty(ticket.ComentariosTicket.Comentario))
                                {
                                    return "Insira um comentário";
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return "Você não tem autorização para atualizar esse ticket";
                            }
                        }
                        else if (ticket.StrAcao == "Pendente" && ticket.Usuario.Funcao.Id != 3)
                        {
                            if (varTicket.Usuario.Id == ticket.Usuario.Id || ticket.Usuario.Funcao.Id != 3) //quem quer atualizar é proprietario ou admin/técnico ? 
                            {//sim o proprietario do ticket quer atualizar.
                                ticket.HistoricoTicket.StatusAnterior = ticket.Status;
                                ticket.Status = "Pendente";
                                ticket.DataAtualizacao = DateTime.Now;
                                if (string.IsNullOrEmpty(ticket.ComentariosTicket.Comentario))
                                {
                                    return "Insira um comentário";
                                }
                            }
                            else
                            {
                                return "Você não tem autorização para atualizar esse ticket";
                            }
                        }
                        else if (ticket.StrAcao == "ResolverChamado" && ticket.Usuario.Funcao.Id != 3 && varTicket.Status == "Em resolução") //necessário verificar se o status é "Em resolução" e o id do usuário é de um técnico ou admin
                        {
                            if (string.IsNullOrEmpty(varTicket.TecnicoResponsavel) || string.IsNullOrEmpty(ticket.TecnicoResponsavel))
                            {
                                return "Para resolver é necessário um técnico atribuído";
                            }
                            else
                            {
                                ticket.HistoricoTicket.StatusAnterior = ticket.Status;
                                ticket.Status = "Resolvido";
                                ticket.DataResolucao = DateTime.Now;
                                ticket.DataAtualizacao = ticket.DataResolucao;

                                if (string.IsNullOrEmpty(ticket.ComentariosTicket.Comentario))
                                {
                                    return "Insira um comentário";
                                }
                            }

                        }
                        else if (ticket.StrAcao == "Cancelar")
                        {
                            if (varTicket.Usuario.Id == ticket.Usuario.Id || ticket.Usuario.Funcao.Id != 3) //quem quer atualizar é proprietario ou admin/técnico ? 
                            {//sim o proprietario do ticket quer atualizar.
                                ticket.HistoricoTicket.StatusAnterior = ticket.Status;
                                ticket.Status = "Cancelado";
                                
                                ticket.DataEncerramento = DateTime.Now;
                                ticket.DataAtualizacao = ticket.DataEncerramento;
                                ticket.DataResolucao = ticket.DataEncerramento; //para o job de SLA não violar o chamado cancelado.

                                if (string.IsNullOrEmpty(ticket.ComentariosTicket.Comentario))
                                {
                                    return "Insira um comentário";
                                }
                            }
                            else
                            {
                                return "Você não tem autorização para atualizar esse ticket";
                            }

                        }

                        return null;
                    } while (varTicket.Status != "Cancelado" || varTicket.Status != "FECHADO");
                }
                else
                {
                    return "Entidade Nula";
                }
            }

        }

        /// <summary>
        /// método que verifica se o usuário colocou um comentário para atualizar a ação...
        /// </summary>
        /// <param name="comentario">Comentário do usuário</param>
        /// <returns>null - comentario inserido</returns>
        public string VerificaComentarioTicket(string comentario)
        {
            if (!string.IsNullOrEmpty(comentario))
            {
                return null;
            }

            return "Insira um comentário";
        }
    }
}
