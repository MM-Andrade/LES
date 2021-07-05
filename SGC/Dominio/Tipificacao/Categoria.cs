using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Categoria : EntidadeDominio
    {
        public string NomeCategoria { get; set; }

        public Categoria()
        {
            NomeCategoria = string.Empty;
        }
    }
}
