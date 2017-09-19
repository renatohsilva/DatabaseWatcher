using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableWatcher;
using TableWatcher.Base;

namespace TableWatcher.Classes
{
    public class Nota
    {
        [AtributoBanco("Handle")]
        public string ID { get; set; }

        [AtributoBanco("NumeroNota")]
        public string Numero { get; set; }

        public List<NotaItem> Items { get; set; }
    }
}
