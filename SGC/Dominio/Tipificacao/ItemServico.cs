using Dominio.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Tipificacao
{
    public class ItemServico : EntidadeDominio
    {
        public string Item { get; set; }
        public Departamento Dpto { get; set; }

        public ItemServico()
        {
            Item = string.Empty;
            Dpto = new Departamento();
        }
    }
}
