using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Usuarios
{
    public class GrupoAtendimento : EntidadeDominio
    {
        public string NomeGrupo { get; set; }
        public Departamento Departamento { get; set; }

        public Usuario Usuario { get; set; }

        public GrupoAtendimento()
        {
            NomeGrupo = string.Empty;
            Departamento = new Departamento();
        }
    }
}
