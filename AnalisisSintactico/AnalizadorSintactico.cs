using Compilador_22023.AnalisisLexico;
using Compilador_22023.GestorErrores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_22023.AnalisisSintactico
{
    public class AnalizadorSintactico
    {

        private AnalizadorLexico AnaLex = new AnalizadorLexico();
        private ComponenteLexico Componente;
        private string falla = "";
        private string causa = "";
        private string solucion = "";
        private Stack<double> pila = new Stack<double>();
        private StringBuilder trazaDepuracion = new StringBuilder();    
        public string Analizar(bool depurar)
        {
            string resultado = "";
            
            DevolverSiguienteComponenteLexico();
            Expresion(0);

            if (ManejadorErrores.ObtenerManejadorErrores().HayErroresAnalisis())
            {
                resultado = "El proceso de compilación finalizó con errores";
            }
            else if (!CategoriaGramatical.FIN_ARCHIVO.Equals(Componente.Categoria) || pila.Count > 1)
            {
                resultado = "Aunque el programa no presenta errores, faltaron componentes por evaluar...";
            }
            else 
            {
                resultado = "El programa se encuentra bien escrito.El resultado de la expresion es: " + pila.Pop();
            }
            if (depurar)
            {
                resultado = resultado + "\n" + trazaDepuracion.ToString();
            }

            return resultado;
        }
        private void DevolverSiguienteComponenteLexico()
        {
            Componente = AnaLex.DevolverSiguienteComponente();
        }
        private void Expresion(int jerarquia)
        {
            FormarInicio("Expresion ", jerarquia);
            Termino(jerarquia + 1);
            ExpresionPrima(jerarquia + 1);
            FormarFin("Expresion ", jerarquia);
        }
        private void ExpresionPrima(int jerarquia)
        {
            FormarInicio("ExpresionPrima ", jerarquia);
            if (EsCategoriaValida(CategoriaGramatical.SUMA))
            {
                DevolverSiguienteComponenteLexico();
                Expresion(jerarquia + 1);

                EvaluarExpresion(CategoriaGramatical.SUMA,jerarquia + 1);
            }
            else if (EsCategoriaValida(CategoriaGramatical.RESTA))
            {
                DevolverSiguienteComponenteLexico();
                Expresion(jerarquia + 1);

                EvaluarExpresion(CategoriaGramatical.RESTA, jerarquia + 1);
            }
            else
            {
                //NADA -> Epsilon
            }
            FormarFin("ExpresionPrima", jerarquia);

        }
        //<termmino>  -> <Factor>
        private void Termino(int jerarquia)
        {
            FormarInicio("Termino ", jerarquia);
            Factor(jerarquia + 1);
            TerminoPrima(jerarquia + 1);
            FormarFin("Termino ", jerarquia);
        }
        private void TerminoPrima(int jerarquia)
        {
            FormarInicio("TerminoPrima ",jerarquia);
            if (EsCategoriaValida(CategoriaGramatical.MULTIPLICACION))
            {
                DevolverSiguienteComponenteLexico();
                Termino(jerarquia + 1);

                EvaluarExpresion(CategoriaGramatical.MULTIPLICACION, jerarquia + 1);
            }
            else if (EsCategoriaValida(CategoriaGramatical.DIVISIÓN))
            {
                DevolverSiguienteComponenteLexico();
                Termino(jerarquia + 1);

                EvaluarExpresion(CategoriaGramatical.DIVISIÓN, jerarquia + 1);
            }
            else
            {
                //NADA
            }
            FormarFin("TerminoPrima ", jerarquia);
        }
        // <Factor>   -> NUMERO_ENTERO{PUSH}
        private void Factor(int jerarquia)
        {
            FormarInicio("Factor", jerarquia);
            if (EsCategoriaValida(CategoriaGramatical.NUMERO_ENTERO))
            {
                pila.Push(Double.Parse(Componente.Lexema));
                FormarPuestaPila(jerarquia + 1);
                DevolverSiguienteComponenteLexico();
            }
            else if (EsCategoriaValida(CategoriaGramatical.NUMERO_DECIMAL))
            {
                pila.Push(Double.Parse(Componente.Lexema));
                FormarPuestaPila(jerarquia + 1);
                DevolverSiguienteComponenteLexico();
            }
            else if (EsCategoriaValida(CategoriaGramatical.PARENTESIS_ABRE))
            {
                DevolverSiguienteComponenteLexico();
                Expresion(jerarquia + 1);
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
            FormarFin("Factor", jerarquia);
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
        private void ReportarErrorSemanticoRecuperable()
        {
            Error error = Error.CREAR_ERROR_SEMANTICO_RECUPERABLE(Componente.NumeroLinea, Componente.PosicionInicial, Componente.Lexema, falla, causa, solucion);
            ManejadorErrores.ObtenerManejadorErrores().ReportarError(error);
        }
        private void EvaluarExpresion(CategoriaGramatical categoria, int jerarquia)
        {
            if (!ManejadorErrores.ObtenerManejadorErrores().HayErroresAnalisis())
            {
                double operandoDerecha = pila.Pop();
                double operandoIzquierda = pila.Pop();

                FormarExtraccionPila(jerarquia,operandoDerecha.ToString());
                FormarExtraccionPila(jerarquia, operandoIzquierda.ToString());

                if (CategoriaGramatical.SUMA.Equals(categoria))
                {
                    pila.Push(operandoDerecha + operandoIzquierda);
                    FormarOperacion(jerarquia, "+", operandoIzquierda.ToString(), operandoDerecha.ToString());
                }
                else if (CategoriaGramatical.RESTA.Equals(categoria))
                {
                    pila.Push(operandoIzquierda - operandoDerecha);
                    FormarOperacion(jerarquia, "-", operandoIzquierda.ToString(), operandoDerecha.ToString());
                }
                else if (CategoriaGramatical.MULTIPLICACION.Equals(categoria))
                {
                    pila.Push(operandoIzquierda * operandoDerecha);
                    FormarOperacion(jerarquia, "*", operandoIzquierda.ToString(), operandoDerecha.ToString());
                }
                else if (CategoriaGramatical.DIVISIÓN.Equals(categoria))
                {
                    FormarOperacion(jerarquia, "/", operandoIzquierda.ToString(), operandoDerecha.ToString());
                    if (operandoDerecha == 0)
                    {
                        falla = "Division por cero";
                        causa = "Se espera un numero diferente de cero para llevar a cabo la division...";
                        solucion = "Asegúrese que en la posición esperada se encuentre un numero diferente de cero";
                        ReportarErrorSemanticoRecuperable();
                        pila.Push(operandoIzquierda / 1);
                    }
                    else
                    {
                        pila.Push(operandoIzquierda / operandoDerecha);
                    }
                }
            }
        }
        private void FormarInicio(String nombreRegla, int jerarquia)
        {
            StringBuilder identador = new StringBuilder();
            for(int indice = 1;indice <= jerarquia;indice++)
            {
                identador.Append("----");
            }
            identador.Append("INICIO REGLA ").Append(nombreRegla);
            identador.Append(" con componente ").Append(Componente.Lexema).Append("\n");

            trazaDepuracion.Append(identador.ToString());
        }
        private void FormarPuestaPila(int jerarquia)
        {
            StringBuilder identador = new StringBuilder();
            for (int indice = 1; indice <= jerarquia; indice++)
            {
                identador.Append("----");
            }
            identador.Append("PUSH ").Append(Componente.Lexema).Append("\n");

            trazaDepuracion.Append(identador.ToString());
        }
        private void FormarExtraccionPila(int jerarquia,string valor)
        {
            StringBuilder identador = new StringBuilder();
            for (int indice = 1; indice <= jerarquia; indice++)
            {
                identador.Append("----");
            }
            identador.Append("POP ").Append(valor).Append("\n");

            trazaDepuracion.Append(identador.ToString());
        }
        private void FormarOperacion(int jerarquia, string operacion,string izquierdo,string derecho)
        {
            StringBuilder identador = new StringBuilder();
            for (int indice = 1; indice <= jerarquia; indice++)
            {
                identador.Append("----");
            }
            identador.Append("OPERANDO ").Append(izquierdo).Append(operacion).Append(derecho).Append("\n");

            trazaDepuracion.Append(identador.ToString());
        }
        private void FormarFin(String nombreRegla, int jerarquia)
        {
            StringBuilder identador = new StringBuilder();
            for (int indice = 1; indice <= jerarquia; indice++)
            {
                identador.Append("----");
            }
            identador.Append("FIN REGLA").Append(nombreRegla);

            trazaDepuracion.Append(identador.ToString());
        }
    }
}
