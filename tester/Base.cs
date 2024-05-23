using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace MatCom.Examen;

/// <summary>
/// Describe una celda de una matriz
/// </summary>
public class CeldaMatriz
{
    /// <summary>
    /// Crea una instancia de la clase CeldaMatriz
    /// </summary>
    /// <param name="fila">Fila donde se encuentra el valor</param>
    /// <param name="columna">Columna donde se encuentra el valor</param>
    /// <param name="valor">Valor</param>
    public CeldaMatriz(int fila, int columna, int valor)
    {
        Valor = valor; Columna = columna; Fila = fila;
    }

    /// <summary>
    /// Especifica la fila de la celda
    /// </summary>
    public int Fila { get; private set; }

    /// <summary>
    /// Especifica la columna de la celda
    /// </summary>
    public int Columna { get; private set; }

    /// <summary>
    /// Especifica o modifica el valor en la celda
    /// </summary>
    public int Valor { get; set; }

    /// <summary> Devuelve la representación literal de una celda </summary>
    /// <returns>Una cadena que representa la celda</returns>
    public override string ToString()
    {
        return string.Format("{0} en ({1};{2})", Valor, Fila, Columna);
    }
}

public interface IMatriz : IEnumerable<CeldaMatriz>
{
    /// <summary>
    /// Devuelve la cantidad de filas de la matriz
    /// </summary>
    int Filas { get; }

    /// <summary>
    /// Devuelve la cantidad de columnas de la matriz
    /// </summary>
    int Columnas { get; }
    /// <summary>
    /// Devuelve la cantidad de elementos distintos de 0
    /// </summary>
    int CantidadDeElementosNoNulos { get; }

    /// <summary>
    /// Inserta una celda en la matriz. Si los valores de fila o columna de la celda
    /// se salen de los límites especificados en el constructor el método debe lanzar
    /// una excepción del tipo IndexOutOfRangeException si ya existía una
    /// celda en esa posición lo que se le hace es cambiar el valor. La inserción
    /// de un valor igual a 0 no debe adicionar una nueva celda, si la celda ya
    /// existía entonces deberá ser eliminada.
    /// </summary>
    /// <param name="celda">Nodo a insertar, provee fila, columna y valor</param>
    void Inserta(CeldaMatriz celda);

    /// <summary>
    /// Devuelve el valor en la fila,columna dada. Si los valores de fila o
    /// columna se salen de los límites especificados en el constructor
    /// el método debe lanzar una excepción del tipo IndexOutOfRangeException. Si
    /// nunca ha sido insertada una celda en dicha fila,columna (o se canceló y eliminó)
    /// se debe retornar el valor 0.
    /// </summary>
    /// <param name="fila">Fila donde se encuentra el valor</param>
    /// <param name="columna">Columna donde se encuentra el valor</param>
    /// <returns>Retorna el valor</returns>
    int ValorEn(int fila, int columna);

    /// <summary>
    /// Realiza la operación de suma entre la matriz actual y la matriz que se
    /// recibe como parámetro, si la operación de suma provoca la inserción de
    /// valores nulos en la matriz, estas celdas deben ser excluidas, exactamente
    /// como cuando se hace en la inserción de un valor nulo
    /// </summary>
    /// <param name="otra">Matriz para sumar con la matriz actual. Esta matriz debe
    /// tener las mismas dimensiones que la matriz actual.</param>
    void Adiciona(IMatriz otra);

    /// <summary>
    /// Itera sobre los elementos de la fila especificada
    /// </summary>
    /// <param name="fila">Fila de la cual se quieren recuperar los elementos</param>
    /// <returns>Retorna la secuencia de elementos distintos de 0 en la fila</returns>
    IEnumerable<CeldaMatriz> Fila(int fila);

    /// <summary>
    /// Itera sobre los elementos de la columna especificada
    /// </summary>
    /// <param name="columna">Columna de la cual se quieren recuperar los elementos
    /// </param>
    /// <returns>Retorna la secuencia de elementos distintos de 0 en la columna</returns>
    IEnumerable<CeldaMatriz> Columna(int columna);
}
