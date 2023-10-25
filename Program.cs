using Compilador_22023.AnalisisLexico;
using Compilador_22023.AnalisisSintactico;
using Compilador_22023.cache;
using Compilador_22023.GestorErrores;
using Compilador_22023.TablaComponentes;

namespace Compilador_22023
{
    class Program
    {
        static void Main(string[] args)
        {
            DataCache.AgregarLinea("5*6");

            AnalizadorSintactico anaSin = new AnalizadorSintactico();

            try
            {
                string respuesta = anaSin.Analizar(true);
                Console.WriteLine(respuesta);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            ImprimirComponentes(TipoComponente.SIMBOLO);
            ImprimirComponentes(TipoComponente.LITERAL);
            ImprimirComponentes(TipoComponente.DUMMY);
            ImprimirComponentes(TipoComponente.PALABRA_RESERVADA);
            ImprimirErrores(TipoError.LEXICO);
            ImprimirErrores(TipoError.SEMANTICO);
            ImprimirErrores(TipoError.SINTACTICO);
            ImprimirErrores(TipoError.GENERADOR_CODIGO_INTERMEDIO);
            ImprimirErrores(TipoError.OPTIMIZACION);
            ImprimirErrores(TipoError.GENERADOR_CODIGO_FINAL);
        }
        private static void ImprimirComponentes(TipoComponente tipo)
        {
            Console.WriteLine("***************INICIO*******"   +tipo.ToString()+ "      **************");

            List<ComponenteLexico> componentes = TablaMaestra.ObtenerTablaMaestra().ObtenerTodosSimbolos(tipo);
            foreach(ComponenteLexico componente in componentes)
            {
                Console.WriteLine(componente.ToString());
            }
            Console.WriteLine("**************** FIN ****" + tipo.ToString() + "  *************");
        }
        private static void ImprimirErrores(TipoError tipo)
        {
            Console.WriteLine("***************INICIO*******" + tipo.ToString() + "      **************");

            List<Error> errores = ManejadorErrores.ObtenerManejadorErrores().ObtenerErrores(tipo);
            foreach (Error error in errores)
            {
                Console.WriteLine(error.ToString());
            }
            Console.WriteLine("**************** FIN ****" + tipo.ToString() + "  *************");
        }
    }
}