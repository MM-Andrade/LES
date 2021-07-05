using Quartz;
using Quartz.Impl;

namespace Core.Jobs
{
    /// <summary>
    /// Classe responsável  por definir e agendar os jobs de acordo com sua periodicidade...
    /// </summary>
    public class JobScheduler
    {
        public async void Start()
        {
            //para versões 3.0 em diante, o job scheduler usa TASK, então vamos usar task assíncronas
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

        
            #region Criador de Jobs
            //informa qual job será criado... no caso esse é o JobVerificaSLA
            IJobDetail verificaSlaViolado = JobBuilder.Create<JobVerificaSlaViolado>().Build();
            IJobDetail encerraTicket = JobBuilder.Create<JobEncerrarTicket>().Build();
            #endregion
            
            #region Definição dos jobs

            //Job para verificar SLA VIOLADO
            //definir um nome para a trigger que inicia o job scheduler
            ITrigger triggerVerificaSlaViolado = TriggerBuilder.Create()   //cria a trigger
                .WithIdentity("TriggerCalcSLA", "GrupoSLA")         //identifica o nome dela e a qual grupo pertence
                .StartNow()     //define que o job deve iniciar imediatamente
                .WithSimpleSchedule(x => x              //sem calendário
                .WithIntervalInSeconds(30)              //eu defini intervalo de 60 segundos para esse job
                .RepeatForever())                       //repetir pra sempre (enquanto a app estiver on)
                .Build();                               //builda a trigger

           await scheduler.ScheduleJob(verificaSlaViolado, triggerVerificaSlaViolado);      //agenda o job de acordo com a trigger que você quer (poderia ser uma outra com calendário, hora definida, etc)


            //JOB para encerrar chamado depois de 3 dias resolvido
            ITrigger triggerEncerraChamado = TriggerBuilder.Create()
                .WithIdentity("TriggerEncChamado", "GrupoChamado")
                .StartNow()
                .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(61)
                .RepeatForever())
                .Build();

            //schedula o job... (por hora não, ele está dando conflito pq starta ao mesmo tempo que o outro...)
            await scheduler.ScheduleJob(encerraTicket, triggerEncerraChamado);

            #endregion

        }
    }
}
