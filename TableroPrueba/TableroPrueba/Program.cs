using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableroPrueba
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Tablero compuesto por R (rows) & C (columns)
             *  °    
             *  x x x x x    R = 5 - Siempre número impar           array (0 -> 4) (0 -> 4)  
             *  x . x x x    C = 5
             *  x . x x x    Pijas -> x 
             *  x x . x x    Pijas faltantes F = (1,1) (2,1) (3,2) -> .
             *  x x x x x
             * _|=|=|=|=|_    Caida celda 
             * 0 1 2 3 4 5    Posiciones 0 y 5 (limites) siempre seran 0
             *       K
             * 
             * Probabilidad pija x              0.5 <-  x  -> 0.5
             * Probabilidad pija faltante . ->          
             * Probabilidad pija x en borde va hacia el centro del tablero | x -> 0,1        x | -> 1,0
             * 
             * K = columna meta -> caida celda que se debe evaluar
             * 
             * Restricciones
             * 
             * 1 < F < 100
             * 3 < R,C < 100
             * 
             * pelota = numero de muestras analizadas
             * 
             */
            Console.WriteLine("Introduzca la ruta del archivo de configuración para el tablero Peg Game:");                        
            string rutaArchivo = Directory.GetCurrentDirectory() + "\\Parametros.txt";
            rutaArchivo = Console.ReadLine();

            //Validamos si el archivo existe
            if (!File.Exists(rutaArchivo))
            { 
                Console.WriteLine("El archivo no existe en la ruta proporcionada, por favor verificar.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("");

            //Obtenemos parametros desde el archivo
            StreamReader objArchivo = new StreamReader(rutaArchivo);
            string linea = "";
            ArrayList arrayTexto = new ArrayList();

            while (linea != null)
            {
                linea = objArchivo.ReadLine();
                if (linea != null)
                    arrayTexto.Add(linea);
            }
            objArchivo.Close();

            int contador = 0;
            int indicepijasfaltantes = 0;
            int _rows = 5;
            int _columns = 5;
            int _numeropijasfaltantes = 3;
            
            int _bola = 1000; //Número de repeticiones 
            int _K = 3;

            _numeropijasfaltantes = arrayTexto.Count - 2;
            string[] pijasfaltantes = new string[_numeropijasfaltantes];

            Console.WriteLine("Parametros:");

            //Poblamos variables validando datos
            foreach (string datos in arrayTexto)
            {
                bool resultado;
                int numero;
                string[] rowcolumns;

                if (contador == 0) //Primera linea obtenemos rows & columns
                {
                    rowcolumns = datos.Split(',');

                    if (rowcolumns.Length <= 1)
                    {
                        Console.WriteLine("El número de filas y columnas no es correcto, por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    resultado = int.TryParse(rowcolumns[0], out numero);
                    if (resultado)
                        _rows = int.Parse(rowcolumns[0]);
                    else
                    {
                        Console.WriteLine("El número de filas (rows) no es un número entero, por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    if (_rows % 2 == 0)
                    {
                        Console.WriteLine("El número de filas (rows) no es un número impar, por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    resultado = int.TryParse(rowcolumns[1], out numero);
                    if (resultado)
                        _columns = int.Parse(rowcolumns[1]);
                    else
                    {
                        Console.WriteLine("El número de columnas (columns) no es un número entero, por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    if(!(_rows > 3 && _rows < 100) || !(_columns > 3 && _columns < 100))
                    {
                        Console.WriteLine("El número de filas (rows) y columnas (columns) deben estar dentro del siguiente rango  3 < (R,C) < 100 , por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    Console.WriteLine("(Rows, Columns) -> " + datos);
                }
                else if (contador == 1) //Segunda linea Meta
                {
                    resultado = int.TryParse(datos, out numero);
                    if (resultado)
                        _K = int.Parse(datos);
                    else
                    {
                        Console.WriteLine("La columna meta (K) no es un número entero, por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    if (!(1 < _K  && _K < _columns))
                    {
                        Console.WriteLine("La columna meta (K) deben estar dentro del siguiente rango  1 < K < columns , por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    Console.WriteLine("Meta (K) -> " + datos);
                }
                else //Demas líneas coordenadas de pijas faltantes
                {
                    rowcolumns = datos.Split(',');

                    if (rowcolumns.Length <= 1)
                    {
                        Console.WriteLine("El número de fila y columna para pija faltante no es correcto, por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    resultado = int.TryParse(rowcolumns[0], out numero);
                    if (!resultado)                    
                    {
                        Console.WriteLine("El renglon " + (contador+1) + " de pija faltante no contiene un número valido, por favor verificar.");
                        Console.ReadLine();
                        return;
                    }
                    
                    if (!(0 < int.Parse(rowcolumns[0]) && int.Parse(rowcolumns[0]) < _rows-1))
                    {
                        Console.WriteLine("El renglon " + (contador + 1) + " de pija faltante no contiene un rango valido para la fila, la fila superior e inferior no tiene pijas faltantes y la fila debe ser 0 < row < (rows-1), por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    resultado = int.TryParse(rowcolumns[1], out numero);
                    if (!resultado)                    
                    {
                        Console.WriteLine("El renglon " + (contador + 1) + " de pija faltante no contiene un número valido, por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    if (!(0 <= int.Parse(rowcolumns[1]) && int.Parse(rowcolumns[1]) < _columns))
                    {
                        Console.WriteLine("El renglon " + (contador + 1) + " de pija faltante no contiene un rango valido para la columna, debe ser 0 < column < columns, por favor verificar.");
                        Console.ReadLine();
                        return;
                    }

                    pijasfaltantes[indicepijasfaltantes] = datos;
                    indicepijasfaltantes++;

                    Console.WriteLine("Pija faltante (row,column) -> " + datos);
                }
                
                contador++;
            }

            Console.WriteLine();

            int[,] _resultado = new int[_columns, _columns + 1];            

            //Realizamos calculos y mostramos resultado
            Tablero(_rows, _columns, pijasfaltantes,ref _resultado, _bola, _K);

            Console.ReadLine();
        } 

        public static void Tablero(int _rows, int _columns, string[] pijasfaltantes,ref int [,] _resultado, int _bola, int _K)
        {
            //Crea tablero
            string[,] tablero = new string[_rows, _columns];

            //Pobla tablero
            for (int x = 0; x < _rows; x++)
                for (int y = 0; y < _columns; y++)
                    tablero[x, y] = "x";

            //Quita pijas
            for (int x = 0; x < pijasfaltantes.Length; x++)
            {
                string[] pijafaltante = pijasfaltantes[x].Split(',');
                tablero[int.Parse(pijafaltante[0]), int.Parse(pijafaltante[1])] = ".";
            }

            Random random = new Random(Environment.TickCount);            

            //Obtiene probabilidades por cada posición partiendo de la columna 0
            for (int posiciones = 0; posiciones < _columns; posiciones++)
            {
                int pelota = _bola;
                
                while (pelota != 0)
                {                    
                    int columna = posiciones;
                    int fila = 0;

                    while (fila < _rows)
                    {
                        //Asignamos probabilidad con respecto al tablero                      
                        if (tablero[fila, columna].Equals("."))
                            fila++;
                        else if (columna == 0)
                        {
                            columna++;                 
                            fila++;
                        }
                        else if (columna == (_columns - 1))
                        {
                            columna--;                            
                            fila++;
                        }                            
                        else
                        {
                            columna += random.Next() % 2 == 0 ? -1 : 1;                            
                            fila++;
                        }                        
                    }
                    _resultado[posiciones, columna]++;
                    pelota--; 
                }                
            }

            //Imprime tablero
            Console.WriteLine("Posiciones");
            for (int y = 0; y < _columns; y++)
                Console.Write(" " + y);

            Console.WriteLine(); Console.WriteLine();
            for (int x = 0; x < _rows; x++)
            {
                for (int y = 0; y < _columns; y++)
                    Console.Write(" " + tablero[x, y]);
                Console.WriteLine();
            }

            Console.WriteLine(); 
            for (int y = 0; y <= _columns; y++)
                Console.Write(y ==0 || y== _columns? "_|": y + "|");
            Console.WriteLine();
            Console.WriteLine("Caida de bola (K)");
            Console.WriteLine();

            //Imprime resultados
            Console.Write(" K " + Convert.ToChar(09));
            for (int x = 1; x < _columns; x++)
                Console.Write(Convert.ToChar(09) + " " + x);
            Console.WriteLine();

            for (int y = 0; y < _columns; y++)
            {
                Console.Write("Posición " + y + Convert.ToChar(09));
                for (int x = 1; x < _columns; x++)
                {                    
                    Console.Write(Math.Round((Convert.ToDecimal(_resultado[y, x]) / _bola) * 100, 2) + "%" + Convert.ToChar(09));
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            //Indica donde es mejor arrojar la bola para alcanzar la meta
            decimal[] porcentajes = new decimal[_columns];
            for (int y = 0; y < _columns; y++)            
                porcentajes[y] = Math.Round((Convert.ToDecimal(_resultado[y, _K]) / _bola) * 100, 2);

            decimal mayor;
            int pos;
            mayor = porcentajes[0];
            pos = 0;
            for (int f = 1; f < porcentajes.Length; f++)
            {
                if (porcentajes[f] > mayor)
                {
                    mayor = porcentajes[f];
                    pos = f;
                }
            }

            Console.WriteLine("Arrojar la bola en la posición " + pos + " te da una probabilidad del " + mayor + "% de caer en la meta K=" + _K);
        }
    }
}
