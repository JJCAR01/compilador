using Compilador_22023.AnalisisLexico;
using Compilador_22023.GestorErrores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_22023.AnalisisSintactico
{
    public class AnalizadorSintactico2
    {
        private AnalizadorLexico AnaLex = new AnalizadorLexico();
        private ComponenteLexico Componente;
        private string falla = "";
        private string causa = "";
        private string solucion = "";

        public string Analizar()
        {
            string resultado = "";
            DevolverSiguienteComponenteLexico();
            Expresion();

            if (ManejadorErrores.ObtenerManejadorErrores().HayErroresAnalisis())
            {
                resultado = "El proceso de compilación finalizó con errores";
            }
            else if (!CategoriaGramatical.FIN_ARCHIVO.Equals(Componente.Categoria))
            {
                resultado = "Aunque el programa no presenta errores, faltaron componentes por evaluar...";
            }
            else
            {
                resultado = "El programa se encuentra bien escrito";
            }

            return resultado;
        }

        private void DevolverSiguienteComponenteLexico()
        {
            Componente = AnaLex.DevolverSiguienteComponente();
        }

        private void Expresion()
        {
            Factor();
            if (EsCategoriaValida(CategoriaGramatical.SUMA))
            {
                DevolverSiguienteComponenteLexico();
            }
            else if(EsCategoriaValida(CategoriaGramatical.RESTA))
            {
                DevolverSiguienteComponenteLexico();
            }
            else if(EsCategoriaValida(CategoriaGramatical.MULTIPLICACION))
            {
                if(EsCategoriaValida(CategoriaGramatical.MULTIPLICACION))
                {
                    Termino();
                }

            }
        }

        private void Termino()
        {
            if(EsCategoriaValida(CategoriaGramatical.MULTIPLICACION))
            {
                DevolverSiguienteComponenteLexico();
                Expresion();
            }
            else if(EsCategoriaValida(CategoriaGramatical.DIVISIÓN))
            {
                DevolverSiguienteComponenteLexico();
                Expresion();
            }
        }

        private void Factor()
        {
            if (EsCategoriaValida(CategoriaGramatical.NUMERO_ENTERO))
            {
                DevolverSiguienteComponenteLexico();
            }
            else if (EsCategoriaValida(CategoriaGramatical.NUMERO_DECIMAL))
            {
                DevolverSiguienteComponenteLexico();
            }
            else if (EsCategoriaValida(CategoriaGramatical.PARENTESIS_ABRE))
            {
                DevolverSiguienteComponenteLexico();
                Expresion();
                if (EsCategoriaValida(CategoriaGramatical.PARENTESIS_CIERRA))
                {
                    DevolverSiguienteComponenteLexico();

                }
                else
                {
                    falla = "Categoría gramática inválida...";
                    causa = "Se esperaba PARÉNTESIS CIERRA, se recibió '" + Componente.Categoria;
                    solucion = "Asegúrese que en la posición esperada se encuentre PARÉNTESIS CIERRA...";
                    ReportarErrorSintacticoStopper();
                }
            }
            else
            {
                falla = "Categoría gramática inválida...";
                causa = "Se esperaba NUMERO ENTERO, NUMERO DECIMAL o PARÉNTESIS ABRE, se recibió '" + Componente.Categoria;
                solucion = "Asegúrese que en la posición esperada se encuentre NUMERO ENTERO, NUMERO DECIMAL o PARÉNTESIS ABRE...";
                ReportarErrorSintacticoStopper();
            }
        }
        private bool EsCategoriaValida(CategoriaGramatical categoria)
        {
            return categoria.Equals(Componente.Categoria);
        }

        private void ReportarErrorSintacticoStopper()
        {
            Error error = Error.CREAR_ERROR_SINTACTICO_STOPPER(Componente.NumeroLinea, Componente.PosicionInicial, Componente.Lexema, falla, causa, solucion);
            ManejadorErrores.ObtenerManejadorErrores().ReportarError(error);
        }
    }
}
