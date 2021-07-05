using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Usuarios
{
    public class Departamento : EntidadeDominio
    {
        public string NomeDepartamento { get; set; }

        public Departamento()
        {
            NomeDepartamento = string.Empty;
        }
    }
}
