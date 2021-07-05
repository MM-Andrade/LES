using Core.Aplicacao;
using Core.Controle;
using Dominio;
using Quartz;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class JobEncerrarTicket : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Ticket ticket = new Ticket();
            Resultado resultado = new Resultado();

            ticket.StrAcao = "JobEncerrarTicket";

            //espera 5 segundos pois não pode iniciar com o job de verifica SLA
            System.Threading.Thread.Sleep(10000);
            System.Diagnostics.Debug.WriteLine("Iniciando job de encerramento de ticket ...");
            


            Task taskAtualiza = new Task(() => resultado = new Fachada().Atualizar(ticket));
            taskAtualiza.Start();

            if (string.IsNullOrEmpty(resultado.Mensagem))
            {
                System.Diagnostics.Debug.WriteLine("Job encerrador de ticket executado...");
                return taskAtualiza;
            }
            else
            {
                //informa no debugger do visual studio (ou IIS) que o job não rodou... (so bad)
                System.Diagnostics.Debug.WriteLine("Job encerrador de ticket não executado...");
                return null;        //apenas para não dar erro
            }
        }
    }
}
