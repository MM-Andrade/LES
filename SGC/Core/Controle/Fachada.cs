using Core.Aplicacao;
using Core.Core;
using Core.DAOs;
using Core.DAOs.EstoqueD;
using Core.DAOs.Tipificacao;
using Core.Negocio;
using Core.Negocio.Chamados;
using Core.Negocio.stChamados;
using Core.Negocio.stEstoque;
using Core.Negocio.stFeedback;
using Core.Negocio.stTipificacao;
using Core.Negocio.StUsuarios;
using Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Controle
{
    public class Fachada : IFachada
    {

        private Resultado resultado;

        private Dictionary<string, IDAO> daos;
        private Dictionary<string, Dictionary<string, List<IStrategy>>> rns;

        public Fachada()
        {
            daos = new Dictionary<string, IDAO>();
            rns = new Dictionary<string, Dictionary<string, List<IStrategy>>>();

            #region Usuario
            
            //Usuario
            daos.Add("Usuario", new UsuarioDAO());
            List<IStrategy> strategiesUsuarioInserir = new List<IStrategy>();
            List<IStrategy> strategiesUsuarioAtualizar = new List<IStrategy>();
            //Inserir
            strategiesUsuarioInserir.Add(new StComplementaDataCadastro());
            strategiesUsuarioInserir.Add(new StInfoMinimaUsuario());
            strategiesUsuarioInserir.Add(new StValidarEmail());
            strategiesUsuarioInserir.Add(new StValidarSenha());
            strategiesUsuarioInserir.Add(new StVerificaUsuarioExistente());
            //Atualziar
            strategiesUsuarioAtualizar.Add(new StAlterarDadosUsuario());
            //strategiesUsuarioAtualizar.Add(new StAlterarSenha());

            Dictionary<string, List<IStrategy>> dictionaryUsuario = new Dictionary<string, List<IStrategy>>();
            dictionaryUsuario.Add("Inserir", strategiesUsuarioInserir);
            dictionaryUsuario.Add("Alterar", strategiesUsuarioAtualizar);
            dictionaryUsuario.Add("Excluir", new List<IStrategy>());
            dictionaryUsuario.Add("Consultar", new List<IStrategy>());
            rns.Add("Usuario", dictionaryUsuario);


            daos.Add("Endereco", new EnderecoDAO());
            List<IStrategy> strategiesEnderecoInserir = new List<IStrategy>();
            //Inserir

            Dictionary<string, List<IStrategy>> dictionaryEndereco = new Dictionary<string, List<IStrategy>>();
            dictionaryEndereco.Add("Inserir", new List<IStrategy>());
            dictionaryEndereco.Add("Alterar", new List<IStrategy>());
            dictionaryEndereco.Add("Excluir", new List<IStrategy>());
            dictionaryEndereco.Add("Consultar", new List<IStrategy>());
            rns.Add("Endereco", dictionaryEndereco);


            //Departamento
            daos.Add("Departamento", new DepartamentoDAO());
            List<IStrategy> strategiesDepartamentos = new List<IStrategy>();

            Dictionary<string, List<IStrategy>> dictionaryDepartamento = new Dictionary<string, List<IStrategy>>();
            dictionaryDepartamento.Add("Inserir", new List<IStrategy>());
            dictionaryDepartamento.Add("Alterar", new List<IStrategy>());
            dictionaryDepartamento.Add("Excluir", new List<IStrategy>());
            dictionaryDepartamento.Add("Consultar", new List<IStrategy>());
            rns.Add("Departamento", dictionaryDepartamento);

            //Função
            daos.Add("Funcao", new FuncaoDAO());
            List<IStrategy> strategiesFuncao = new List<IStrategy>();

            Dictionary<string, List<IStrategy>> dictionaryFuncao = new Dictionary<string, List<IStrategy>>();
            dictionaryFuncao.Add("Inserir", new List<IStrategy>());
            dictionaryFuncao.Add("Alterar", new List<IStrategy>());
            dictionaryFuncao.Add("Excluir", new List<IStrategy>());
            dictionaryFuncao.Add("Consultar", new List<IStrategy>());
            rns.Add("Funcao", dictionaryFuncao);

            //Função
            daos.Add("GrupoAtendimento", new GrupoAtendimentoDAO());

            Dictionary<string, List<IStrategy>> dictionaryGrupo = new Dictionary<string, List<IStrategy>>();
            dictionaryGrupo.Add("Inserir", new List<IStrategy>());
            dictionaryGrupo.Add("Alterar", new List<IStrategy>());
            dictionaryGrupo.Add("Excluir", new List<IStrategy>());
            dictionaryGrupo.Add("Consultar", new List<IStrategy>());
            rns.Add("GrupoAtendimento", dictionaryGrupo);

            #endregion

            #region Tipificacao

            //Prioridades
            daos.Add("Prioridade", new PrioridadeDAO());
            List<IStrategy> strategiesProridadeInserir = new List<IStrategy>();

            Dictionary<string, List<IStrategy>> dictionaryPrioridade = new Dictionary<string, List<IStrategy>>();
            dictionaryPrioridade.Add("Inserir", new List<IStrategy>());
            dictionaryPrioridade.Add("Alterar", new List<IStrategy>());
            dictionaryPrioridade.Add("Excluir", new List<IStrategy>());
            dictionaryPrioridade.Add("Consultar", new List<IStrategy>());
            rns.Add("Prioridade", dictionaryPrioridade);

            //Tipos de Serviço
            daos.Add("TipoServico", new TipoServicoDAO());
            List<IStrategy> strategiesTipoServicoInserir = new List<IStrategy>();
            List<IStrategy> strategiesTipoServicoAtualizar = new List<IStrategy>();

            //Inserir
            strategiesTipoServicoInserir.Add(new StComplementaDataCadastro());
            strategiesTipoServicoInserir.Add(new StValidaCamposServico());
            strategiesTipoServicoInserir.Add(new StVerificaServicoExistente());

            //Atualizar
            strategiesTipoServicoAtualizar.Add(new StDesativaServico());


            Dictionary<string, List<IStrategy>> dictionaryTipoServico = new Dictionary<string, List<IStrategy>>();
            dictionaryTipoServico.Add("Inserir", strategiesTipoServicoInserir);
            dictionaryTipoServico.Add("Alterar", strategiesTipoServicoAtualizar);
            dictionaryTipoServico.Add("Excluir", new List<IStrategy>());
            dictionaryTipoServico.Add("Consultar", new List<IStrategy>());
            rns.Add("TipoServico", dictionaryTipoServico);


            //ItemServico
            daos.Add("ItemServico", new ItemServicoDAO());
         

            Dictionary<string, List<IStrategy>> dictionaryItemServico = new Dictionary<string, List<IStrategy>>();

            List<IStrategy> strategiesitemservicoinserir = new List<IStrategy>();
            strategiesitemservicoinserir.Add(new stAdicionaItemServico());

            dictionaryItemServico.Add("Inserir", strategiesitemservicoinserir);
            dictionaryItemServico.Add("Alterar", new List<IStrategy>());
            dictionaryItemServico.Add("Excluir", new List<IStrategy>());
            dictionaryItemServico.Add("Consultar", new List<IStrategy>());
            rns.Add("ItemServico", dictionaryItemServico);

            #endregion

            #region Estoque
            daos.Add("Estoque", new EstoqueDAO());
            List<IStrategy> strategiesESAtualizar = new List<IStrategy>();
            List<IStrategy> strategiesESInserir = new List<IStrategy>();
            strategiesESInserir.Add(new stValidaItemEstoque());
            strategiesESInserir.Add(new stVerificaEanExistente());
            strategiesESAtualizar.Add(new stBaixaEstoqueChamado());
            strategiesESAtualizar.Add(new stAtualizaQuantidadeEstoque());


            Dictionary<string, List<IStrategy>> dictionaryEstoque = new Dictionary<string, List<IStrategy>>();
            dictionaryEstoque.Add("Inserir", strategiesESInserir);
            dictionaryEstoque.Add("Alterar", strategiesESAtualizar);
            dictionaryEstoque.Add("Excluir", new List<IStrategy>());
            dictionaryEstoque.Add("Consultar", new List<IStrategy>());
            rns.Add("Estoque", dictionaryEstoque);


            daos.Add("AlteracaoEstoque", new AlteracaoEstoqueDAO());
            List<IStrategy> strategiesAlteracaoEstoqueInserir = new List<IStrategy>();
            strategiesAlteracaoEstoqueInserir.Add(new stAlteracaoEstoqueChamado());
            strategiesAlteracaoEstoqueInserir.Add(new StComplementaDataCadastro());


            Dictionary<string, List<IStrategy>> dictionaryAlteracaoEstoque = new Dictionary<string, List<IStrategy>>();
            dictionaryAlteracaoEstoque.Add("Inserir", strategiesAlteracaoEstoqueInserir);
            dictionaryAlteracaoEstoque.Add("Alterar", new List<IStrategy>());
            dictionaryAlteracaoEstoque.Add("Excluir", new List<IStrategy>());
            dictionaryAlteracaoEstoque.Add("Consultar", new List<IStrategy>());
            rns.Add("AlteracaoEstoque", dictionaryAlteracaoEstoque);

            #endregion

            #region Ticket

            daos.Add("Ticket", new TicketDAO());
            List<IStrategy> strategiesTicketInserir = new List<IStrategy>();
            List<IStrategy> strategiesTicketAtualizar = new List<IStrategy>();

            strategiesTicketInserir.Add(new stNovoChamado());
            strategiesTicketInserir.Add(new stGeraNumeroChamado());
            strategiesTicketAtualizar.Add(new StAtualizaTicket());
            strategiesTicketAtualizar.Add(new StEncerrarTicket());
            strategiesTicketAtualizar.Add(new stResolverTicket());

            Dictionary<string, List<IStrategy>> dictionaryTicket = new Dictionary<string, List<IStrategy>>();
            dictionaryTicket.Add("Inserir", strategiesTicketInserir);
            dictionaryTicket.Add("Alterar", strategiesTicketAtualizar);
            dictionaryTicket.Add("Excluir", new List<IStrategy>());
            dictionaryTicket.Add("Consultar", new List<IStrategy>());
            rns.Add("Ticket", dictionaryTicket);

            daos.Add("SlaTicket", new SlaTicketDAO());
            List<IStrategy> strategiesSLATicketInserir = new List<IStrategy>();
            strategiesSLATicketInserir.Add(new stCalculaSLA());

            Dictionary<string, List<IStrategy>> dictionarySLATicket = new Dictionary<string, List<IStrategy>>();
            dictionarySLATicket.Add("Inserir", strategiesSLATicketInserir);
            dictionarySLATicket.Add("Alterar", new List<IStrategy>());
            dictionarySLATicket.Add("Excluir", new List<IStrategy>());
            dictionarySLATicket.Add("Consultar", new List<IStrategy>());
            rns.Add("SlaTicket", dictionarySLATicket);

            daos.Add("Feedback", new FeedbackDAO());
            List<IStrategy> strategiesFeedbackAtualizar = new List<IStrategy>();
            strategiesFeedbackAtualizar.Add(new stAtualizaFeedback());

            Dictionary<string, List<IStrategy>> dictionaryFeedback = new Dictionary<string, List<IStrategy>>();
            dictionaryFeedback.Add("Inserir", new List<IStrategy>());
            dictionaryFeedback.Add("Alterar", strategiesFeedbackAtualizar);
            dictionaryFeedback.Add("Excluir", new List<IStrategy>());
            dictionaryFeedback.Add("Consultar", new List<IStrategy>());
            rns.Add("Feedback", dictionaryFeedback);

            daos.Add("ComentariosTicket", new ComentariosTicketDAO());
            List<IStrategy> strategiesComentariosInserir = new List<IStrategy>();
            strategiesComentariosInserir.Add(new StComplementaDataCadastro());

            Dictionary<string, List<IStrategy>> dictionaryComentarios = new Dictionary<string, List<IStrategy>>();
            dictionaryComentarios.Add("Inserir", strategiesComentariosInserir);
            dictionaryComentarios.Add("Alterar", new List<IStrategy>());
            dictionaryComentarios.Add("Excluir", new List<IStrategy>());
            dictionaryComentarios.Add("Consultar", new List<IStrategy>());
            rns.Add("ComentariosTicket", dictionaryComentarios);

            daos.Add("HistoricoTicket", new HistoricoTicketDAO());
            List<IStrategy> strategiesHistoricoInserir = new List<IStrategy>();

            Dictionary<string, List<IStrategy>> dictionaryHistorico = new Dictionary<string, List<IStrategy>>();
            dictionaryHistorico.Add("Inserir", new List<IStrategy>());
            dictionaryHistorico.Add("Alterar", new List<IStrategy>());
            dictionaryHistorico.Add("Excluir", new List<IStrategy>());
            dictionaryHistorico.Add("Consultar", new List<IStrategy>());
            rns.Add("HistoricoTicket", dictionaryHistorico);



            daos.Add("ServicosUtilizados", new ServicosUtilizadosDAO());
            List<IStrategy> strategiesSUinserir = new List<IStrategy>();
            //strategiesSUinserir.Add(new stAtribuiTicketServico());

            Dictionary<string, List<IStrategy>> dictionarySU = new Dictionary<string, List<IStrategy>>();
            dictionarySU.Add("Inserir", new List<IStrategy>());
            dictionarySU.Add("Alterar", new List<IStrategy>());
            dictionarySU.Add("Excluir", new List<IStrategy>());
            dictionarySU.Add("Consultar", new List<IStrategy>());
            rns.Add("ServicosUtilizados", dictionarySU);

            #endregion

            #region Análise
            daos.Add("AnaliseTicket", new AnaliseTicketDAO());
            Dictionary<string, List<IStrategy>> dictionaryAnaliseTicket = new Dictionary<string, List<IStrategy>>();

            List<IStrategy> consultarAnalise = new List<IStrategy>();
            consultarAnalise.Add(new stAnalise());

            dictionaryAnaliseTicket.Add("Inserir", new List<IStrategy>());
            dictionaryAnaliseTicket.Add("Alterar", new List<IStrategy>());
            dictionaryAnaliseTicket.Add("Excluir", new List<IStrategy>());
            dictionaryAnaliseTicket.Add("Consultar", consultarAnalise);
            rns.Add("AnaliseTicket", dictionaryAnaliseTicket);

            #endregion

        }
        public Resultado Atualizar(EntidadeDominio entidade)
        {
            resultado = new Resultado();
            string nomeClasse = entidade.GetType().Name;
            string mensagem = ExecutarRegras(entidade, "Alterar");

            if (mensagem == null)
            {
                IDAO dao = daos[nomeClasse.ToString()];
                try
                {
                    dao.Atualizar(entidade);
                    List<IEntidade> entidades = new List<IEntidade>();
                    entidades.Add(entidade);
                    resultado.Entidades = entidades;
                }
                catch (Exception ex)
                {
                    resultado.Mensagem = ex.Message;
                    throw;
                }
            }
            else
            {
                resultado.Mensagem = mensagem;
            }

            return resultado;
        }

        public Resultado Consultar(IEntidade entidade)
        {
            resultado = new Resultado();
            string nmClasse = entidade.GetType().Name;
            //string mensagem = ExecutarRegras(entidade, "Consultar");

            IDAO dao = daos[nmClasse.ToString()];
                try
                {
                    resultado.Entidades = dao.Consultar(entidade);
                }
                catch (Exception ex)
                {
                    resultado.Mensagem = ex.Message;
                }
           
            return resultado;
        }

        public Resultado Excluir(EntidadeDominio entidade)
        {
            resultado = new Resultado();
            string nomeClasse = entidade.GetType().Name;
            string mensagem = ExecutarRegras(entidade, "Excluir");

            if (mensagem == null)
            {
                IDAO dao = daos[nomeClasse.ToString()];
                try
                {
                    dao.Excluir(entidade);
                    List<IEntidade> entidades = new List<IEntidade>();
                    entidades.Add(entidade);
                    resultado.Entidades = entidades; ;
                }
                catch (Exception ex)
                {
                    resultado.Mensagem = ex.Message;
                    throw;
                }
            }
            else
            {
                resultado.Mensagem = mensagem;
            }

            return resultado;
        }

        public Resultado Inserir(EntidadeDominio entidade)
        {
            resultado = new Resultado();
            string nomeClasse = entidade.GetType().Name;
            string mensagem = ExecutarRegras(entidade, "Inserir");

            if (mensagem == null)
            {
                IDAO dao = daos[nomeClasse.ToString()];
                try
                {
                    dao.Inserir(entidade);
                    List<IEntidade> entidades = new List<IEntidade>();
                    entidades.Add(entidade);
                    resultado.Entidades = entidades;
                }
                catch (Exception ex)
                {
                    resultado.Mensagem = ex.Message;
                    throw ex;
                }
            }
            else
            {
                resultado.Mensagem = mensagem;
            }

            return resultado;
        }

        private string ExecutarRegras(EntidadeDominio entidade, string operacao)
        {
            string nomeClasse = entidade.GetType().Name;
            StringBuilder msg = new StringBuilder();
            Dictionary<string, List<IStrategy>> regrasOperacao = rns[nomeClasse.ToString()];

            if (regrasOperacao != null)
            {
                List<IStrategy> regras = regrasOperacao[operacao.ToString()];
                if (regras != null)
                {
                    foreach (IStrategy s in regras)
                    {
                        string m = s.Processar(entidade);
                        if (m != null)
                        {
                            msg.Append(m);
                            msg.Append("\n");
                        }
                    }
                }
            }

            if (msg.Length > 0)
                return msg.ToString();
            else
                return null;
        }
    }
}
