using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Usuarios
{
    public class Funcao : EntidadeDominio
    {
        public string NomeFuncao { get; set; }
        public bool GerenciarUsuario { get; set; }
        public bool AtendimentoTecnico { get; set; }
        public bool CriarCategoria { get; set; }
        public bool CriarRelatorio { get; set; }
        public bool VisualizarRelatorio { get; set; }
    }
}
