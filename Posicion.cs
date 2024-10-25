using System;

namespace Snake2D
{
    public struct Posicion
    {
        public int X { get; set; } //COORDENADA HORIZONTAL
        public int Y { get; set; } //COORDENADA VERTICAL


        //EN EL CONSTRUCTOR RECOGEMOS DATOS DE LA CREACIÓN DE LOS OBJETOS.
        //POR EJEMPLO:
        // serpienteJugador1 = new List<Posicion>
        //        {
        //            new Posicion(10, 10), //CABEZA
        //            new Posicion(9, 10),  //CUERPO 1
        //            new Posicion(8, 10)   //CUERPO 3
        //        };
        //direccionJugador1 = new Posicion(1, 0); //SU DESPLZMIENTO EMPIEZA HACIA LA DERECHA
    public Posicion(int x, int y)
        {
            X = x;
            Y = y;
        }

        //PERMITE SUMAR POSICIONES
        //MODIFICAMOS LA FORMA EN LA QUE C# SUMA, GRACIAS A ESTE MÉTODO QUE SE ENCARGA DE SUMAR DOS "POSICION".
        //ES DECIR, LA UBICACIÓN Y LA DIRECCIÓN 
        //SIN DEFINIR operator + C# MOSTRARÁ UN ERROR PORQUE NO SABE CÓMO SUMAR DOS POSICION
        //DE ESTA FORMA, TOMAMMOS (a y b de X) y (a y b de Y) PARA OBTENER LA NUEVA COORDENADA X Y LA NUEVA COORDENADA Y
        //DEVOLVIENDO EL OBJETO POSICIÓN CON 2 ATRIBUTOS
        public static Posicion operator +(Posicion a, Posicion b)
        {
            return new Posicion(a.X + b.X, a.Y + b.Y);
        }

        //COMPARAR POSICIONES, TAMBIÉN LOS 3 SERÁN SOBRECARGADOS PARA COMPARAR X+Y Y X+Y.
        public static bool operator ==(Posicion a, Posicion b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        //COMPARAR POSICIONES
        public static bool operator !=(Posicion a, Posicion b)
        {
            return !(a == b);
        }
        //COMPARAR POSICIONES
        public override bool Equals(object obj)
        {
            if (obj is Posicion)
            {
                Posicion other = (Posicion)obj;
                return this == other;
            }
            return false;
        }

        //ESTE MÉTODO GENERA UNA ESPECIE DE IDENTIFICADOR ÚNICO E IRREPETIBLE APRA CADA POSICIÓN, SON CÓIDIGOS DEL HASH
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
