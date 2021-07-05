using Dominio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Frontend.Helpers
{
    /// <summary>
    /// Classe que contém os métodos para auxilio na geração do gráfico (análise)
    /// Baseado na classe do aluno Julio Cesar Alves - Sistema PodeChamar - 2018
    /// </summary>
    public class Grafico
    {
        public static List<Linha> GraficoTickets(List<Ticket> tickets, int ano)
        {
            List<Linha> linhas = new List<Linha>();
            List<string> legendas = new List<string>();

            int anoGrafico = 0;

            foreach (Ticket tipo in tickets)
            {
                legendas.Add(tipo.TipoServico.NomeServico);
            }

            foreach (string leg in legendas)
            {
                Linha linha = new Linha();
                foreach (Ticket ticket in tickets)
                {
                    anoGrafico = ticket.DataCadastro.Year;
                    if (ticket.TipoServico.NomeServico == leg && anoGrafico == ano)
                    {
                        int mes = ticket.DataCadastro.Month;
                        linha.Valores[mes -1]++;
                    }
                }

                linha.Legenda = leg;
                linhas.Add(linha);
            }
            return linhas;

        }
    }

    public class Linha
    {

        public string Legenda { get; set; }

        public double[] Valores { get; set; }

        //public int Meses { get; set; }

        public Linha()
        {
            
            //Meses = 12;

            //inicializa o vetor com 12 posições
            Valores = new double[12];

            ////inicializa o vetor de valores a serem colocados na linha
            //for (int i = 0; i < Meses; i++)
            //{
            //    Valores[i] = 0;
            //}
        }
    }
}