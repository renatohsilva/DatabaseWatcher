using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableWatcher;

namespace TableWatcher.Classes
{
    public class Pessoa
    {
        public int Handle { get; set; }

        [AtributoBanco("Nome")]
        public string NomePessoa { get; set; }

        public int Idade { get; set; }
    }
}
