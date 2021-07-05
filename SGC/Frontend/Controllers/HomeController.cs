using Core.Aplicacao;
using Core.Controle;
using Dominio;
using Dominio.Chamados;
using Dominio.Tipificacao;
using Dominio.Usuarios;
using Frontend.Helpers;
using Frontend.Models;
using Highsoft.Web.Mvc.Charts;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        Resultado resultado;
        Usuario usuario;

        #region Actions Comuns
        public ActionResult Login()
        {
            if (Session["Usuario"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario usuario)
        {
            usuario.StrAcao = "Login";

            if (ModelState.IsValid)
            {
                resultado = new Fachada().Consultar(usuario);
            }
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] += "\n" + resultado.Mensagem;
                ViewBag.Usuario = usuario;

                return View("Login");
            }

            Session["Usuario"] = (Usuario)resultado.Entidades.FirstOrDefault();
            usuario = (Usuario)resultado.Entidades.FirstOrDefault(); ;
            Session["idpermissao"] = usuario.Funcao.Id;


            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        #endregion

        #region Actions de Chamados
        public ActionResult MeusChamados()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Ticket ticket = new Ticket();

            usuario = (Usuario)Session["Usuario"]; //recupera o usuário logado...
            ticket.Usuario.Id = usuario.Id; //passa o id do usuário logado para poder pesquisar...

            resultado = new Fachada().Consultar(ticket);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] += "\n" + resultado.Mensagem;


                return View();
            }

            List<Ticket> tickets = new List<Ticket>();
            foreach (var item in resultado.Entidades)
                tickets.Add(item as Ticket);

            ViewBag.Tickets = tickets;
            return View();
        }

        public ActionResult MinhaFila()
        { //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Ticket ticket = new Ticket();

            usuario = (Usuario)Session["Usuario"]; //recupera o usuário logado...
            ticket.Usuario.Id = usuario.Id; //passa o id do usuário logado para poder pesquisar...
            ticket.Usuario.GrupoAtendimento.Departamento.Id = usuario.GrupoAtendimento.Departamento.Id; //atribui o dpto...
            ticket.StrAcao = "MinhaFila";

            resultado = new Fachada().Consultar(ticket);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] += "\n" + resultado.Mensagem;


                return View();
            }

            List<Ticket> tickets = new List<Ticket>();
            foreach (var item in resultado.Entidades)
                tickets.Add(item as Ticket);

            ViewBag.Tickets = tickets;
            return View();
        }

        public ActionResult TodosChamados()
        { //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Ticket ticket = new Ticket();

            usuario = (Usuario)Session["Usuario"]; //recupera o usuário logado...
            ticket.Usuario.Id = usuario.Id; //passa o id do usuário logado para poder pesquisar...
            ticket.StrAcao = "BuscaAdmin";

            resultado = new Fachada().Consultar(ticket);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] += "\n" + resultado.Mensagem;


                return View();
            }

            List<Ticket> tickets = new List<Ticket>();
            foreach (var item in resultado.Entidades)
                tickets.Add(item as Ticket);

            ViewBag.Tickets = tickets;
            return View();
        }

        public ActionResult AbrirChamado(int idservico = 0)
        {

            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            else if (idservico <= 0)
            {
                return RedirectToAction("Catalogo", "Home");
            }
            else
            {
                TempData["idservico"] = idservico;

                usuario = (Usuario)Session["Usuario"];
                ViewBag.Usuario = usuario;

                TipoServico tipoServico = new TipoServico();
                tipoServico.Id = idservico;

                Resultado resultado = new Fachada().Consultar(tipoServico);
                ViewBag.TipoServico = resultado.Entidades.FirstOrDefault();

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AbrirChamado(Ticket ticket)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            usuario = (Usuario)Session["Usuario"];

            //ticket.Usuario.Id = usuario.Id;
            ticket.Usuario = usuario;
            resultado = new Fachada().Inserir(ticket);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }
            else
            {
                TempData["MensagemSucesso"] = "Chamado " + ticket.CodigoTicket + " aberto com sucesso!!!";
                return RedirectToAction("MeusChamados", "Home");
            }

        }
        [HttpGet]
        public ActionResult Ticket(string id)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            Ticket ticket = new Ticket();
            ticket.CodigoTicket = id;

            usuario = (Usuario)Session["Usuario"];
            ViewBag.Usuario = usuario;

            resultado = new Fachada().Consultar(ticket);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }

            ViewBag.Ticket = resultado.Entidades.FirstOrDefault() as Ticket;
            Session["Ticket"] = resultado.Entidades.FirstOrDefault() as Ticket; //Salva o ticket na session... (Apenas para buscar os comentários e histórico :) )
            return View();
        }
        [HttpPost]
        public ActionResult Ticket(Ticket ticket)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            usuario = (Usuario)Session["Usuario"];
            ViewBag.Usuario = usuario;      //(se arrancar ele da nullpointer nas ações...!
            ticket.Usuario = usuario;

            if (ticket.StrAcao != "Resolver")
            {
                resultado = new Fachada().Atualizar(ticket);
                if (!string.IsNullOrEmpty(resultado.Mensagem))
                {
                    ViewBag.Ticket = ticket;
                    TempData["MensagemErro"] = resultado.Mensagem;
                    return View();
                }
                else
                {
                    ViewBag.Ticket = resultado.Entidades.FirstOrDefault() as Ticket;
                    Session["Ticket"] = resultado.Entidades.FirstOrDefault() as Ticket; //Salva o ticket na session... (Apenas para buscar os comentários e histórico :) )
                    TempData["MensagemSucesso"] = "Ticket atualizado";

                    return RedirectToAction("Ticket", "Home", ticket.CodigoTicket);
                }

            }
            else
            {
                Session["ResolverTicket"] = ticket;
                return RedirectToAction("ResolverChamado", "Home");
            }

        }
        public ActionResult Catalogo()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Resultado resultado = new Fachada().Consultar(new TipoServico());

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }

            List<TipoServico> tipoServicos = new List<TipoServico>();
            foreach (var item in resultado.Entidades)
                tipoServicos.Add(item as TipoServico);

            ViewBag.Servicos = tipoServicos;

            return View();
        }
        [HttpPost]
        public ActionResult Catalogo(string strBusca)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            TipoServico tipoServico = new TipoServico();
            List<TipoServico> tipoServicos = new List<TipoServico>();

            Resultado resultado = new Resultado();
            tipoServico.StrBusca = strBusca;              //recebe o termo de busca

            resultado = new Fachada().Consultar(tipoServico);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }
            else
            {
                foreach (var item in resultado.Entidades)
                {
                    tipoServicos.Add(item as TipoServico);

                }
                if (tipoServicos.Count > 0)
                {
                    ViewBag.Servicos = tipoServicos;
                }
                else
                {
                    TempData["MensagemErro"] = "Sem tipificações para o termo informado";
                    return View();
                }
            }
            return View();
        }

        public ActionResult ResolverChamado()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Ticket ticket = new Ticket();
            ticket = (Ticket)Session["ResolverTicket"];
            ViewBag.Ticket = ticket;

            //variavel para buscar itens de estoque e de serviço de acordo com o id do departamento
            int idDepartamento;
            idDepartamento = ticket.TipoServico.Departamento.Id;

            ViewBag.ItemServico = RecuperaItensServicoResolucao(idDepartamento);
            ViewBag.ItemEstoque = RecuperaItensEstoqueResolucao(idDepartamento);

            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticket">objeto a ser salvo no bd</param>
        /// <param name="form">formcollection com os nomes das inputs para salvar</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResolverChamado(Ticket ticket, FormCollection form)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            usuario = (Usuario)Session["Usuario"];

            ticket.Usuario = usuario;
            ticket.StrAcao = ControllerContext.RouteData.Values["action"].ToString();

            //recebe o valor de formcollection dos multiselects do frontend
            //eles vem como string com virgula, então temmos que quebrar ele (split)
            if (!string.IsNullOrEmpty(form["ItemServicoForm"]))
            {
                string[] itemServico = form["ItemServicoForm"].Split(',');
                //agora precisamos preencher as listas do objeto ticket com os ids dos itens de serviços
                foreach (var item in itemServico)
                {
                    ticket.ItensServico.Add(Convert.ToInt32(item));
                }
            }

            if (!string.IsNullOrEmpty(form["ItemEstoqueForm"]))
            {
                string[] ItemEstoque = form["ItemEstoqueForm"].Split(',');
                foreach (var item in ItemEstoque)
                {
                    ticket.ItensEstoque.Add(Convert.ToInt32(item));
                }
            }

            //finalmente atualizar o chamado e fechar ele :)
            resultado = new Fachada().Atualizar(ticket);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                int idDepartamento;
                idDepartamento = ticket.TipoServico.Departamento.Id;
                ViewBag.ItemServico = RecuperaItensServicoResolucao(idDepartamento);
                ViewBag.ItemEstoque = RecuperaItensEstoqueResolucao(idDepartamento);

                ViewBag.Ticket = ticket;
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }


            TempData["MensagemSucesso"] = "Ticket: " + ticket.CodigoTicket + " resolvido com sucesso";
            return RedirectToAction("MinhaFila", "Home");
        }

        public ActionResult ComentariosTicket()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Ticket ticket = new Ticket();
                Resultado resultado = new Resultado();

                ComentariosTicket comentario = new ComentariosTicket();
                List<ComentariosTicket> comentarios = new List<ComentariosTicket>();

                ticket = (Ticket)Session["Ticket"];     //recupera ticket da session...

                comentario.CodigoTicket = ticket.CodigoTicket;

                resultado = new Fachada().Consultar(comentario);

                if (!string.IsNullOrEmpty(resultado.Mensagem))
                {
                    TempData["Mensagem"] = resultado.Mensagem;
                    return View();
                }
                else
                {
                    foreach (var item in resultado.Entidades)
                    {
                        comentarios.Add(item as ComentariosTicket);

                    }
                    if (comentarios.Count > 0)
                    {
                        ViewBag.Comentarios = comentarios;
                    }
                    else
                    {
                        TempData["Mensagem"] = "Sem comentários para exibir...";
                        return View();
                    }

                }
                return View();
            }

        }
        public ActionResult HistoricoTicket()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Ticket ticket = new Ticket();
                Resultado resultado = new Resultado();

                HistoricoTicket historicoTicket = new HistoricoTicket();
                List<HistoricoTicket> historicoTickets = new List<HistoricoTicket>();

                ticket = (Ticket)Session["Ticket"];     //recupera ticket da session...

                historicoTicket.CodigoTicket = ticket.CodigoTicket;

                resultado = new Fachada().Consultar(historicoTicket);

                if (!string.IsNullOrEmpty(resultado.Mensagem))
                {
                    TempData["Mensagem"] = resultado.Mensagem;
                    return View();
                }
                else
                {
                    foreach (var item in resultado.Entidades)
                    {
                        historicoTickets.Add(item as HistoricoTicket);

                    }
                    if (historicoTickets.Count > 0)
                    {
                        ViewBag.Historico = historicoTickets;
                    }
                    else
                    {
                        TempData["Mensagem"] = "Sem histórico para exibir...";
                        return View();
                    }

                }
                return View();
            }
        }
        public ActionResult ExibeSlaTicket()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Resultado resultado = new Resultado();

                Ticket ticket = new Ticket();
                SlaTicket slaTicket = new SlaTicket();
                slaTicket.Ticket = new Ticket();

                List<Ticket> slaTickets = new List<Ticket>();

                ticket = (Ticket)Session["Ticket"];     //recupera ticket da session...

                slaTicket.Ticket.CodigoTicket = ticket.CodigoTicket;

                resultado = new Fachada().Consultar(slaTicket);

                if (!string.IsNullOrEmpty(resultado.Mensagem))
                {
                    TempData["Mensagem"] = resultado.Mensagem;
                    return View();
                }
                else
                {
                    foreach (var item in resultado.Entidades)
                    {
                        slaTickets.Add(item as Ticket);

                    }
                    if (slaTickets.Count > 0)
                    {
                        ViewBag.SLA = slaTickets;
                    }
                    else
                    {
                        TempData["Mensagem"] = "Sem SLA para exibir...";
                        return View();
                    }

                }
                return View();
            }
        }

        public ActionResult MeusFeedbacks()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Resultado resultado = new Resultado();

            Ticket ticket = new Ticket();
            Feedback feedback = new Feedback();

            usuario = (Usuario)Session["Usuario"]; //recupera o usuário logado...
            feedback.Ticket.Usuario.Id = usuario.Id; //passa o id do usuário logado para poder pesquisar...
            feedback.StrAcao = "ConsultaFeedbackUsuario";

            resultado = new Fachada().Consultar(feedback);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] += "\n" + resultado.Mensagem;


                return View();
            }

            List<Feedback> feedbacks = new List<Feedback>();
            foreach (var item in resultado.Entidades)
                feedbacks.Add(item as Feedback);

            ViewBag.Feedbacks = feedbacks;
            return View();

        }
        public ActionResult InfoFeedback(string codigoticket)
        {

            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Resultado resultado = new Resultado();

            Feedback feedback = new Feedback();

            usuario = (Usuario)Session["Usuario"]; //recupera o usuário logado...

            feedback.Ticket.CodigoTicket = codigoticket; //passa o codigo do ticket para enviar ao dao.
            feedback.StrAcao = "VisualizarFeedback";

            resultado = new Fachada().Consultar(feedback);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] += "\n" + resultado.Mensagem;


                return View();
            }
            else
            {
                ViewBag.Feedbacks = resultado.Entidades.FirstOrDefault() as Feedback;

                return View();
            }

        }
        public ActionResult AtualizarFeedback(string codigoticket)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Resultado resultado = new Resultado();

            Ticket ticket = new Ticket();
            Feedback feedback = new Feedback();

            usuario = (Usuario)Session["Usuario"]; //recupera o usuário logado...

            feedback.Ticket.CodigoTicket = codigoticket; //passa o codigo do ticket para enviar ao dao.
            feedback.StrAcao = "VisualizarFeedback";

            resultado = new Fachada().Consultar(feedback);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] += "\n" + resultado.Mensagem;


                return View();
            }
            else
            {
                ViewBag.Feedbacks = resultado.Entidades.FirstOrDefault() as Feedback;

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AtualizarFeedback(Feedback feedback)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            resultado = new Fachada().Atualizar(feedback);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                ViewBag.Ticket = feedback;
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }
            else
            {
                ViewBag.Feedback = resultado.Entidades.FirstOrDefault() as Feedback;
                TempData["MensagemSucesso"] = "Obrigado pelo feedback";

                return RedirectToAction("MeusFeedbacks", "Home");
            };
        }
        public ActionResult Feedbacks()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Resultado resultado = new Resultado();

            Ticket ticket = new Ticket();
            Feedback feedback = new Feedback();

            usuario = (Usuario)Session["Usuario"]; //recupera o usuário logado...
            feedback.Ticket.Usuario.Id = usuario.Id; //passa o id do usuário logado para poder pesquisar...
            feedback.StrAcao = "ConsultaFeedbackAdm";

            resultado = new Fachada().Consultar(feedback);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] += "\n" + resultado.Mensagem;


                return View();
            }

            List<Feedback> feedbacks = new List<Feedback>();
            foreach (var item in resultado.Entidades)
                feedbacks.Add(item as Feedback);

            ViewBag.Feedbacks = feedbacks;
            return View();
        }


        public ActionResult ServicosUtilizadosTicket()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Resultado resultado = new Resultado();

                Ticket ticket = new Ticket();
                ServicosUtilizados servicosUtilizados = new ServicosUtilizados();
                List<ServicosUtilizados> servicos = new List<ServicosUtilizados>();

                ticket = (Ticket)Session["Ticket"];     //recupera ticket da session...
                servicosUtilizados.GetTicket.Id = ticket.Id;

                resultado = new Fachada().Consultar(servicosUtilizados);

                if (!string.IsNullOrEmpty(resultado.Mensagem))
                {
                    TempData["Mensagem"] = resultado.Mensagem;
                    return View();
                }
                else
                {
                    foreach (var item in resultado.Entidades)
                    {
                        servicos.Add(item as ServicosUtilizados);

                    }
                    ViewBag.ServicosUtilizados = servicos;
                }
                return View();
            }
        }
        #endregion

        #region Tipificação de serviços
        public ActionResult Servicos()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Resultado resultado = new Fachada().Consultar(new TipoServico());

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }

            List<TipoServico> tipoServicos = new List<TipoServico>();
            foreach (var item in resultado.Entidades)
                tipoServicos.Add(item as TipoServico);

            ViewBag.Servicos = tipoServicos;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Servicos(string strBusca)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            TipoServico tipoServico = new TipoServico();
            List<TipoServico> tipoServicos = new List<TipoServico>();

            Resultado resultado = new Resultado();
            tipoServico.StrBusca = strBusca;              //recebe o termo de busca

            resultado = new Fachada().Consultar(tipoServico);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }
            else
            {
                foreach (var item in resultado.Entidades)
                {
                    tipoServicos.Add(item as TipoServico);

                }
                if (tipoServicos.Count > 0)
                {
                    ViewBag.Servicos = tipoServicos;
                }
                else
                {
                    TempData["MensagemErro"] = "Sem tipificações para o termo informado";
                    return View();
                }
            }
            return View();

        }
        public ActionResult AdicionarServico()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamento = RecuperaDepartamento();
            ViewBag.Prioridades = RecuperaPrioridade();


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdicionarServico(TipoServico tipoServico)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamento = RecuperaDepartamento();
            ViewBag.Prioridades = RecuperaPrioridade();

            Resultado resultado = new Fachada().Inserir(tipoServico);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                ViewBag.TipoServico = tipoServico;
                return View(tipoServico);
            }
            TempData["MensagemSucesso"] = "Tipificação incluída com sucesso";
            return RedirectToAction("Servicos", "Home");
        }
        public ActionResult DesativarServico(string StrAcao, int id)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            TipoServico tipoServico = new TipoServico();

            tipoServico.Id = id;
            tipoServico.StrAcao = StrAcao;


            Resultado resultado = new Fachada().Atualizar(tipoServico);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }
            TempData["MensagemSucesso"] = "Tipificação desativada";
            return RedirectToAction("Servicos", "Home");
        }
        public ActionResult InfoServico(int id)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamentos = RecuperaDepartamento();
            ViewBag.Prioridades = RecuperaPrioridade();

            TipoServico tipoServico = new TipoServico();

            tipoServico.Id = id;

            Resultado resultado = new Fachada().Consultar(tipoServico);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;

                return View();
            }

            ViewBag.TipoServico = resultado.Entidades.FirstOrDefault();

            return View();
        }
        public ActionResult EditarServico(int id)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamentos = RecuperaDepartamento();
            ViewBag.Prioridades = RecuperaPrioridade();


            TipoServico tipoServico = new TipoServico();

            tipoServico.Id = id;

            Resultado resultado = new Fachada().Consultar(tipoServico);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }

            ViewBag.TipoServico = resultado.Entidades.FirstOrDefault();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarServico(TipoServico tipoServico, int id)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamento = RecuperaDepartamento();
            ViewBag.Prioridades = RecuperaPrioridade();

            tipoServico.Id = id;

            Resultado resultado = new Fachada().Atualizar(tipoServico);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }
            return RedirectToAction("Servicos", "Home");
        }

        public ActionResult ItensDeServico()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Resultado resultado = new Fachada().Consultar(new ItemServico());

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }

            List<ItemServico> tipoServicos = new List<ItemServico>();
            foreach (var item in resultado.Entidades)
                tipoServicos.Add(item as ItemServico);

            ViewBag.Servicos = tipoServicos;

            return View();

        }
        [HttpPost]
        public ActionResult ItensDeServico(string strbusca)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ItemServico itemServico = new ItemServico();
            List<ItemServico> itemServicos = new List<ItemServico>();

            Resultado resultado = new Resultado();
            itemServico.StrBusca = strbusca;              //recebe o termo de busca

            resultado = new Fachada().Consultar(itemServico);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }
            else
            {
                foreach (var item in resultado.Entidades)
                {
                    itemServicos.Add(item as ItemServico);

                }
                if (itemServicos.Count > 0)
                {
                    ViewBag.Servicos = itemServicos;
                }
                else
                {
                    TempData["MensagemErro"] = "Sem tipificações para o termo informado";
                    return View();
                }
            }
            return View();
        }

        public ActionResult ExcluirItemdeServico(int id)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ItemServico itemServico = new ItemServico()
            {
                Id = id         //passa o ID para exclusão
            };

            Resultado resultado = new Fachada().Excluir(itemServico);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return RedirectToAction("ItensDeServico", "Home");
            }
            else
            {
                TempData["MensagemErro"] = "Item de serviço " + itemServico.Item + " excluído";
                return RedirectToAction("ItensDeServico", "Home");
            }
         
        }

        public ActionResult AdicionarItemDeServico()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamento = RecuperaDepartamento();


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdicionarItemDeServico(ItemServico itemServico)
        { //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamento = RecuperaDepartamento();

            resultado = new Fachada().Inserir(itemServico);

            if(string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemSucesso"] = "Item de serviço cadastrado com sucesso";
                return RedirectToAction("ItensDeServico", "Home");
            }
            else
            {
                TempData["MensagemErro"] += resultado.Mensagem;
                return View(itemServico);
            }

        }

        #endregion

        #region Actions de Usuários
        public ActionResult Usuarios()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Resultado resultado = new Fachada().Consultar(new Usuario());

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }

            List<Usuario> usuarios = new List<Usuario>();
            foreach (var item in resultado.Entidades)
                usuarios.Add(item as Usuario);

            ViewBag.Usuarios = usuarios;

            return View();
        }
        [HttpPost]
        public ActionResult Usuarios(string strBusca)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Usuario usuario = new Usuario();
            List<Usuario> usuarios = new List<Usuario>();

            Resultado resultado = new Resultado();
            usuario.StrBusca = strBusca;              //recebe o termo de busca

            resultado = new Fachada().Consultar(usuario);
            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }
            else
            {
                foreach (var item in resultado.Entidades)
                {
                    usuarios.Add(item as Usuario);

                }
                if (usuarios.Count > 0)
                {
                    ViewBag.Usuarios = usuarios;
                }
                else
                {
                    TempData["MensagemErro"] = "Sem usuários para o termo pesquisado";
                    return View();
                }
            }
            return View();
        }
        public ActionResult NovoUsuario()
        {

            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.GrupoAtendimento = RecuperaGrupoAtendimento();
            ViewBag.Funcao = RecuperarFuncao();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovoUsuario(Usuario usuario)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.GrupoAtendimento = RecuperaGrupoAtendimento();
            ViewBag.Funcao = RecuperarFuncao();

            Resultado resultado = new Fachada().Inserir(usuario);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                ViewBag.Usuario = usuario;
                return View();
            }
            TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso";
            return RedirectToAction("Usuarios", "Home");
        }
        public ActionResult InfoUsuario(int id = 0)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            Resultado resultado = new Resultado();
            Usuario usuario = new Usuario();

            usuario.Id = id;

            resultado = new Fachada().Consultar(usuario);

            //preenche viewbag para exibição do registro
            ViewBag.Usuarios = resultado.Entidades.FirstOrDefault() as Usuario;

            return View();
        }
        public ActionResult EditarUsuario(int id)
        {

            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.GrupoAtendimento = RecuperaGrupoAtendimento();
            ViewBag.Funcao = RecuperarFuncao();

            Resultado resultado = new Resultado();
            Usuario usuario = new Usuario();

            usuario.Id = id;
            TempData["idusuario"] = usuario.Id;

            //se for pesquisa o perfil...
            resultado = new Fachada().Consultar(usuario);

            //preenche viewbag para exibição do registro
            ViewBag.Usuario = resultado.Entidades.FirstOrDefault() as Usuario;

            Usuario usrSession = new Usuario();
            usrSession = (Usuario)Session["Usuario"];
            ViewBag.Usuarios = usrSession;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarUsuario(Usuario usuario)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            Usuario usrSession = new Usuario();

            usrSession = (Usuario)Session["Usuario"];
            usuario.Id = Convert.ToInt32(TempData["idusuario"]);

            //Recebe o valor do nome da actionresult para Editar o usuário
            usuario.StrAcao = ControllerContext.RouteData.Values["action"].ToString();


            ViewBag.Usuarios = usrSession;

            ViewBag.GrupoAtendimento = RecuperaGrupoAtendimento();
            ViewBag.Funcao = RecuperarFuncao();



            Resultado resultado = new Fachada().Atualizar(usuario);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                ViewBag.Usuario = usuario;
                return View();
            }
            TempData["MensagemSucesso"] = "Usuário atualizado com sucesso";
            return RedirectToAction("Usuarios", "Home");
        }
        public ActionResult MeuPerfil()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            usuario = (Usuario)Session["Usuario"];

            //se for pesquisa o perfil...
            resultado = new Fachada().Consultar(usuario);

            //preenche viewbag para exibição do registro
            ViewBag.Usuarios = resultado.Entidades.FirstOrDefault() as Usuario;

            return View();

        }
        public ActionResult AlterarSenha()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View();
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlterarSenha(Usuario usuario)
        {   //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Usuario usuarioS = new Usuario();
            usuarioS = (Usuario)Session["Usuario"];

            usuario.Id = usuarioS.Id;
            usuario.Email = usuarioS.Email;
            usuario.StrAcao = ControllerContext.RouteData.Values["action"].ToString(); //para indicar qual ação dentro do DAO Atualizar deve ser feita...

            Resultado resultado = new Fachada().Atualizar(usuario);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = "Não foi possível alterar a senha, tente novamente \n" +
                    resultado.Mensagem;
                return View();
            }
            else
            {
                TempData["MensagemSucesso"] = "Senha alterada com sucesso!";
                return View();
            }
        }
        public ActionResult GruposDeAtendimento()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            return View();
        }
        #endregion

        #region Actions de Estoque
        public ActionResult Estoque()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            Resultado resultado = new Fachada().Consultar(new Estoque());

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }

            List<Estoque> estoque = new List<Estoque>();
            foreach (var item in resultado.Entidades)
                estoque.Add(item as Estoque);

            ViewBag.Estoque = estoque;

            return View();

        }
        [HttpPost]
        public ActionResult Estoque(string strBusca)
        {
            //Valida Usuário Logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Estoque estoque = new Estoque();
                List<Estoque> itensestoque = new List<Estoque>();

                estoque.StrBusca = strBusca;

                Resultado resultado = new Fachada().Consultar(estoque);

                foreach (var item in resultado.Entidades)
                {
                    itensestoque.Add(item as Estoque);

                }
                if (itensestoque.Count > 0)
                {
                    ViewBag.Estoque = itensestoque;
                }
                else
                {
                    TempData["MensagemErro"] = "Sem itens para o termo pesquisado";
                    return View();
                }
            }
            return View();
        }

        public ActionResult NovoProduto()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamento = RecuperaDepartamento();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovoProduto(Estoque estoque)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamento = RecuperaDepartamento();

            Resultado resultado = new Fachada().Inserir(estoque);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                ViewBag.Estoque = estoque;
                return View();
            }
            TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso";
            return RedirectToAction("Estoque", "Home");
        }

        public ActionResult InfoProduto(int id)
        {

            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            Resultado resultado = new Resultado();
            Estoque estoque = new Estoque();

            estoque.Id = id;

            resultado = new Fachada().Consultar(estoque);

            //preenche viewbag para exibição do registro
            ViewBag.Estoque = resultado.Entidades.FirstOrDefault() as Estoque;

            return View();
        }
        public ActionResult EditarProduto(int id)
        {

            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamento = RecuperaDepartamento();

            Resultado resultado = new Resultado();
            Estoque estoque = new Estoque();
            estoque.Id = id;

            resultado = new Fachada().Consultar(estoque);

            //preenche viewbag para exibição do registro
            ViewBag.Estoque = resultado.Entidades.FirstOrDefault() as Estoque;
            TempData["estoque"] = resultado.Entidades.FirstOrDefault() as Estoque;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarProduto(Estoque estoque)
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            Usuario usrSession = new Usuario();

            usrSession = (Usuario)Session["Usuario"];
            estoque.GetUsuario.Id = usrSession.Id;

            estoque.StrAcao = ControllerContext.RouteData.Values["action"].ToString(); //para indicar qual ação dentro do DAO Atualizar deve ser feita...
            Resultado resultado = new Fachada().Atualizar(estoque);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                ViewBag.Estoque = TempData["estoque"];
                return View();
            }
            TempData["MensagemSucesso"] = "Item "+estoque.Item+ " atualizado com sucesso";
            return RedirectToAction("Estoque", "Home");

        }

        public ActionResult IntensEstoqueUtilizadosTicket()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Resultado resultado = new Resultado();

                Ticket ticket = new Ticket();
                AlteracaoEstoque alteracaoEstoque = new AlteracaoEstoque();
                List<AlteracaoEstoque> itensEstoque = new List<AlteracaoEstoque>();

                ticket = (Ticket)Session["Ticket"];     //recupera ticket da session...
                alteracaoEstoque.Motivo = ticket.CodigoTicket;

                resultado = new Fachada().Consultar(alteracaoEstoque);

                if (!string.IsNullOrEmpty(resultado.Mensagem))
                {
                    TempData["Mensagem"] = resultado.Mensagem;
                    return View();
                }
                else
                {
                    foreach (var item in resultado.Entidades)
                    {
                        itensEstoque.Add(item as AlteracaoEstoque);

                    }
                    ViewBag.AlteracaoEstoque = itensEstoque;
                }
                return View();
            }
        }

        #endregion

        #region SLA

        public ActionResult SLAs()
        { //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            SlaTicket slaTicket = new SlaTicket();
            slaTicket.StrAcao = ControllerContext.RouteData.Values["action"].ToString(); //para indicar qual ação dentro do DAO Atualizar deve ser feita...

            Resultado resultado = new Fachada().Consultar(slaTicket);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                TempData["MensagemErro"] = resultado.Mensagem;
                return View();
            }



            List<Ticket> slaTickets = new List<Ticket>();
            foreach (var item in resultado.Entidades)
                slaTickets.Add(item as Ticket);

            ViewBag.SLA = slaTickets;

            return View();
        }
        [HttpPost]
        public ActionResult SLAs(string strbusca)
        {
            //Valida Usuário Logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                SlaTicket slaTicket = new SlaTicket();
                List<Ticket> tickets = new List<Ticket>();

                slaTicket.StrBusca = strbusca;
                slaTicket.StrAcao = ControllerContext.RouteData.Values["action"].ToString(); //para indicar qual ação dentro do DAO deve ser feita...
                Resultado resultado = new Fachada().Consultar(slaTicket);

                foreach (var item in resultado.Entidades)
                {
                    tickets.Add(item as Ticket);

                }
                if (tickets.Count > 0)
                {
                    ViewBag.SLA = tickets;
                }
                else
                {
                    TempData["MensagemErro"] = "Sem itens para o termo pesquisado";
                    return View();
                }
            }
            return View();
        }

        #endregion


        #region Actions de Relatorios / Análise
        public ActionResult Relatorios()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Departamento = RecuperaDepartamento();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Relatorio(AnaliseTicket analiseTicket)
        {
            //Valida Usuário Logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.Analise = analiseTicket;

            //TempData["Analise"] = analiseTicket;

            List<Ticket> tickets = new List<Ticket>();
            List<TipoServico> tipoServicos = new List<TipoServico>();
            int ano = 0;

            //analiseTicket = (AnaliseTicket)TempData["Analise"];
            resultado = new Fachada().Consultar(analiseTicket);

            foreach (AnaliseTicket analise in resultado.Entidades)
            {
                tickets.Add(analise.GetTicket);
                tipoServicos.Add(analise.GetTicket.TipoServico);
            }
            //atribui o ano de incio de pesquisa
            ano = analiseTicket.DataFinalAnalise.Year;

            //por fim, preenche as linhas para o gráfico
            List<Linha> linhas = Grafico.GraficoTickets(tickets,ano);



            //teste pra entender como o grafico funciona
            //Linha Energia = linhas.Where(i => i.Legenda.Equals("Queda de energia")).FirstOrDefault();
            //List<double> energiaValues = new List<double>();
            //foreach (var item in Energia.Valores)
            //{
            //    double d = Convert.ToDouble(item);
            //    energiaValues.Add(d);
            //}

            //ViewData["energia"] = energiaValues;



            List<double> categoriasValues = new List<double>(); //valores das legendas a serem preenchidos
            List<string> legendaLabels = new List<string>(); //labels das legendas
            IEnumerable<Linha> legendasView = new List<Linha>();    //para enviar as legendas para o frontend...

            //percorre a lista de linhas geradas pelo método de ordenação
            foreach (var item in linhas)
            {
                if (item.Legenda.Equals(item.Legenda))
                {
                    categoriasValues = item.Valores.ToList();
                    //cria a linha do gráfico e seus valores
                    List<LineSeriesData> legendaData = new List<LineSeriesData>();
                    categoriasValues.ForEach(p => legendaData.Add(new LineSeriesData { Y = p }));
                    ViewData[item.Legenda] = legendaData;
                }

            }

          
            //seleciona um item de cada legenda e o atribui la lista para enviar ao front.
            legendasView = linhas.DistinctBy(t => t.Legenda);    
            
            //atribui o valor para enviar na view dentro de labels
            foreach (var item in legendasView)
            {
                legendaLabels.Add(item.Legenda);
            }

            //SHAZAM coroy!
            ViewBag.Legendas = legendaLabels;
            return View();
        }

        public ActionResult Dashboard()
        {
            //Valida usuário logado...
            if (Session["Usuario"] == null)
            {
                TempData["MensagemErro"] += "É necessário estar logado para executar essa ação!";
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        //método não utilizado... mas deixa ele aqui pq é interessante (funciona se souber usar, não foi o meu caso)
        public JsonResult RelatorioJson()
        {
            List<object> iData = new List<object>();

            AnaliseTicket analiseTicket = new AnaliseTicket();

            analiseTicket = (AnaliseTicket)TempData["Analise"];

            resultado = new Fachada().Consultar(analiseTicket);

            if (!string.IsNullOrEmpty(resultado.Mensagem))
            {
                RedirectToAction("Relatorios", "Home");
                TempData["MensagemErro"] += "\n" + resultado.Mensagem;

                return null;
            }


            //para preencher o datatable a ser enviado para o grafico...
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Contagem", Type.GetType("System.Int32"));
            dataTable.Columns.Add("TipoServicoNome", Type.GetType("System.String"));
            dataTable.Columns.Add("Data", Type.GetType("System.String"));


            foreach (AnaliseTicket ticket in resultado.Entidades)
            {
                DataRow dr = dataTable.NewRow();
                dr["Contagem"] = ticket.ContadorTipo;
                dr["TipoServicoNome"] = ticket.GetTicket.TipoServico.NomeServico;
                dr["Data"] = Convert.ToString(ticket.Data);
                dataTable.Rows.Add(dr);
            }


            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dataTable.Rows select drr[dataColumn.ColumnName]).ToList();
                iData.Add(x);
            }

            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Helpers
        public List<TipoServico> RecuperaTipoServico()
        {
            TipoServico tipoServico = new TipoServico();
            Resultado resultado = new Fachada().Consultar(tipoServico);

            List<TipoServico> tipoServicos = new List<TipoServico>();

            foreach (TipoServico servico in resultado.Entidades)
            {
                tipoServicos.Add(servico);
            }
            return tipoServicos.ToList();
        }

        public List<Departamento> RecuperaDepartamento()
        {
            Departamento departamento = new Departamento();
            Resultado resultado = new Fachada().Consultar(departamento);

            List<Departamento> listDepartamento = new List<Departamento>();

            foreach (Departamento dpto in resultado.Entidades)
            {
                listDepartamento.Add(dpto);
            }
            return listDepartamento.ToList();
        }

        public List<GrupoAtendimento> RecuperaGrupoAtendimento()
        {
            GrupoAtendimento grupoAtendimento = new GrupoAtendimento();
            Resultado resultado = new Fachada().Consultar(grupoAtendimento);

            List<GrupoAtendimento> listGrupo = new List<GrupoAtendimento>();

            foreach (GrupoAtendimento gpAtendimento in resultado.Entidades)
            {
                listGrupo.Add(gpAtendimento);
            }
            return listGrupo.ToList();
        }

        public List<Funcao> RecuperarFuncao()
        {
            Funcao funcao = new Funcao();
            Resultado resultado = new Fachada().Consultar(funcao);

            List<Funcao> listFuncao = new List<Funcao>();

            foreach (Funcao func in resultado.Entidades)
            {
                listFuncao.Add(func);
            }
            return listFuncao.ToList();
        }

        public List<Prioridade> RecuperaPrioridade()
        {
            Prioridade prioridade = new Prioridade();
            Resultado resultado = new Fachada().Consultar(prioridade);

            List<Prioridade> prioridades = new List<Prioridade>();

            foreach (Prioridade pri in resultado.Entidades)
            {
                prioridades.Add(pri);
            }
            return prioridades.ToList();
        }

        public List<ItemServico> RecuperaItensServicoResolucao(int idDepartamento)
        {
            ItemServico itemServico = new ItemServico();

            itemServico.Dpto.Id = idDepartamento;

            Resultado resultado = new Fachada().Consultar(itemServico);

            List<ItemServico> itemServicos = new List<ItemServico>();

            foreach (ItemServico itens in resultado.Entidades)
            {
                itemServicos.Add(itens);
            }
            return itemServicos.ToList();
        }
        public List<ItemServico> RecuperaItensServico(int idTicket)
        {
            ItemServico itemServico = new ItemServico();
            Resultado resultado = new Fachada().Consultar(itemServico);

            List<ItemServico> itemServicos = new List<ItemServico>();

            foreach (ItemServico itens in resultado.Entidades)
            {
                itemServicos.Add(itens);
            }
            return itemServicos.ToList();
        }

        public List<Estoque> RecuperaItensEstoqueResolucao(int idDepartamento)
        {
            Estoque estoque = new Estoque();

            estoque.Departamento.Id = idDepartamento;
            Resultado resultado = new Fachada().Consultar(estoque);

            List<Estoque> itensEstoque = new List<Estoque>();

            foreach (Estoque est in resultado.Entidades)
            {
                itensEstoque.Add(est);
            }

            return itensEstoque.ToList();
        }

        //public List<Estoque> RecuperaItensEstoque()
        //{
        //    Estoque estoque = new Estoque();
        //    Resultado resultado = new Fachada().Consultar(estoque);

        //    List<Estoque> itensEstoque = new List<Estoque>();

        //    foreach (Estoque est in resultado.Entidades)
        //    {
        //        itensEstoque.Add(est);
        //    }

        //    return itensEstoque.ToList();
        //}

        public ActionResult Voltar(string returnUrl)
        {
            if (returnUrl != "")
            {
                return Redirect(returnUrl);
            }
            return View();
        }


        public ActionResult Sair()
        {
            if (Session["Usuario"] != null)
            {
                Session.Clear();
                Session.Abandon();

                TempData["MensagemSucesso"] = "Você saiu do sistema...";
                return RedirectToAction("Login", "Home");
            }
            return RedirectToAction("Login", "Home");

        }



        #endregion

    }




}