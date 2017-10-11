using TableWatcher.Helper;

namespace BennerESocialDbWatcherService.Model
{
    public class Paises
    {
        public int HANDLE { get; set; }

        [AtributoESocial("CODIGOEXPORTACAO")]
        public int CodigoESocial { get; set; }

        [AtributoESocial("NOME")]
        public string NomePais { get; set; }

        public string Gentilico { get; set; }
    }
}
