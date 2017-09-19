using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableWatcher
{
    public class AtributoBanco : Attribute
    {
        public string NomeBanco;

        public AtributoBanco(string nomeBanco)
        {
            this.NomeBanco = nomeBanco;
        }
    }
}
