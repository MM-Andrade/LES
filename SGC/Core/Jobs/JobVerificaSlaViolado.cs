using Core.Aplicacao;
using Core.Controle;
using Dominio;
using Quartz;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class JobVerificaSlaViolado : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {

            SlaTicket slaTicket = new SlaTicket();
            Resultado resultado = new Resultado();

            slaTicket.StrAcao = "JobVerificaSlaViolado";

            System.Diagnostics.Debug.WriteLine("Iniciando job de SLA VIOLADO...");

            Task taskAtualiza = new Task(() => resultado = new Fachada().Atualizar(slaTicket));
            taskAtualiza.Start();

            if (string.IsNullOrEmpty(resultado.Mensagem))
            {
                System.Diagnostics.Debug.WriteLine("Job de SLA VIOLADO executado...");
                return taskAtualiza;
            }
            else
            {
                //informa no debugger do visual studio (ou IIS) que o job não rodou... (so bad)
                System.Diagnostics.Debug.WriteLine("Job de SLA VIOLADO não executado...");
                return null;        //apenas para não dar erro
            }
                
        }
    }
}
