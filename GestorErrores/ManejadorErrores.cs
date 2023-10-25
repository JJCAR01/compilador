using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_22023.GestorErrores
{
    public class ManejadorErrores
    {
        private Dictionary<TipoError, List<Error>> errores = new Dictionary<TipoError, List<Error>>();
        private static readonly ManejadorErrores INSTANCIA = new ManejadorErrores();

        private ManejadorErrores()
        {
            Limpiar();
        }

        public static ManejadorErrores ObtenerManejadorErrores()
        {
            return INSTANCIA;
        }

        private void Limpiar() 
        {
            errores.Add(TipoError.LEXICO,new List<Error>());
            errores.Add(TipoError.OPTIMIZACION, new List<Error>());
            errores.Add(TipoError.SINTACTICO, new List<Error>());
            errores.Add(TipoError.SEMANTICO, new List<Error>());
            errores.Add(TipoError.GENERADOR_CODIGO_INTERMEDIO, new List<Error>());
            errores.Add(TipoError.GENERADOR_CODIGO_FINAL, new List<Error>());
        }
        public void ReportarError(Error error)
        {
            if(error != null)
            {
                errores[error.Tipo].Add(error);
                if (CategoriaError.STOPPER.Equals(error.Categoria))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("********************Error Tipo STOPPER*******************").Append("\r\n");
                    sb.Append("Se ha presentado un error de tipo Stopper dentro del proceso").Append(error.Tipo).Append("\r\n");
                    sb.Append("Verifique la tabla de errores para tener mayor detalle").Append("\r\n");

                    throw new Exception(sb.ToString());
                }
            }
        }
        public bool HayErrores(TipoError tipo) 
        {
            return errores.ContainsKey(tipo) && errores[tipo].Count > 0;

        }
        public bool HayErroresAnalisis()
        {
            return HayErrores(TipoError.LEXICO) || HayErrores(TipoError.SINTACTICO) || HayErrores(TipoError.SEMANTICO);

        }
        public bool HayErroresSintesis()
        {
            return HayErrores(TipoError.GENERADOR_CODIGO_INTERMEDIO) || HayErrores(TipoError.OPTIMIZACION) || HayErrores(TipoError.GENERADOR_CODIGO_FINAL);

        }
        public bool HayErroresCompilacion()
        {
            return HayErroresAnalisis() || HayErroresSintesis();

        }
        public List<Error> ObtenerErrores(TipoError tipo) 
        {
            return errores[tipo];
        }

    }
}
