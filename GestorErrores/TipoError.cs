using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_22023.GestorErrores
{
    public enum TipoError
    {
        LEXICO,SEMANTICO,SINTACTICO,GENERADOR_CODIGO_INTERMEDIO,OPTIMIZACION,GENERADOR_CODIGO_FINAL
    }
}
