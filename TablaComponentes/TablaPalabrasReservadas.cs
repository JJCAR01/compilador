using Compilador_22023.AnalisisLexico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_22023.TablaComponentes
{
    public class TablaPalabrasReservadas
    {
        private Dictionary<string, List<ComponenteLexico>> tabla = new Dictionary<string, List<ComponenteLexico>>();
        private Dictionary<string, ComponenteLexico> palabrasReservadas = new Dictionary<string, ComponenteLexico>();

        public TablaPalabrasReservadas() 
        {
            LllenarPalabrasReservadas();
        }

        private void LllenarPalabrasReservadas()
        {
            palabrasReservadas.Add("class", ComponenteLexico.CREAR_PALABRA_RESERVADA(0, 0, "class", CategoriaGramatical.PALABRA_RESERVADA_CLASS));
            palabrasReservadas.Add("if", ComponenteLexico.CREAR_PALABRA_RESERVADA(0, 0, "if", CategoriaGramatical.PALABRA_RESERVADA_IF));

        }
        public void Limpiar()
        {
            tabla.Clear();
        }

        public void ComprobarPalabraReservada(ComponenteLexico componente)
        {
            if(componente != null && palabrasReservadas.ContainsKey(componente.Lexema)) 
            {
                componente.Categoria = palabrasReservadas[componente.Lexema].Categoria;
                componente.Tipo = TipoComponente.PALABRA_RESERVADA;
            }
        }

        public void Agregar(ComponenteLexico componente)
        {
            if (componente != null && TipoComponente.PALABRA_RESERVADA.Equals(componente.Tipo))
            {
                ObtenerSimbolo(componente.Lexema).Add(componente);
            }
        }

        public List<ComponenteLexico> ObtenerSimbolo(string lexema)
        {
            if (tabla.ContainsKey(lexema))
            {
                tabla.Add(lexema, new List<ComponenteLexico>());
            }
            return tabla[lexema];
        }
        public List<ComponenteLexico> ObtenerTodosSimbolos()
        {
            List<ComponenteLexico> listarRetorno = new List<ComponenteLexico>();

            foreach (List<ComponenteLexico> lista in tabla.Values)
            {
                listarRetorno.AddRange(lista);
            }
            return listarRetorno;
        }
    }
}
