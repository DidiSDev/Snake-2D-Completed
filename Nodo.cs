using System;

namespace Snake2D
{
    public class Nodo
    {
        //AQUÍ TENEMOS LA CHICHA DEL ALGORITMO A* PARA FACILITARNOS UN POCO LA VIDA
        public Posicion Posicion { get; set; } //UBICACIÓN DEL NODO EN EL TABLERO
        //DEFINIMOS X E Y DONDE SE ENCUENTRA ESTE NODO
        public Nodo Padre { get; set; }
        //REPRESENTA EL NODO ANTERIOR O EL NODO ORIGEN EN EL CAMINO
        //RECONSTRUIMOS LA RUTA TOMADA HACIA LA MANZANA UNA VEZ SE ENCUENTRA
        //AYUDA A RASTREAR EL CAMINO DESDE EL NODO OBJETIVO AL NODO INICIO, ASÍ VEMOS LA SECUENCIA
        //DE MOVIMIENTOS NECESARIOS PARA LLEGAR
        public int G { get; set; } // COSTO *****ACUMULADO***** DESDE EL INICIO HASTA ESTE NODO
        public int H { get; set; } //HEURISTICA (DISTANCIA HASTA EL OBJETIVO SUMANDO LAS DIFERENCIAS ABSOLUTAS
        //ENTRE LAS COORDENADAS X E Y DEL NODO ACTUAL Y EL OBJETIVO
        public int F => G + H;     // COSTO TOTAL, A MENOR VALOR DE  F, MEJOR CAMINO

        public Nodo(Posicion posicion, Nodo padre, int g, int h)
        {
            Posicion = posicion;
            Padre = padre;
            G = g;
            H = h;
        }
    }
}
