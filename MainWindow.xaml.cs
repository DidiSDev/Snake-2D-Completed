using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Snake2D
{
    public partial class MainWindow : Window
    {
        //HABILITAMOS LOS ATRIBUTOS DE LAS SERPIENTES DE JUGADOR1,2 Y LA IA, SUS RESPECTIVAS POSICIONES, UN INTERRUPTOR QUE VERIFICA CUANDO HA TERMINADO LA PARTIDA
        //EL TAMAÑO DEL CUERPO DE CADA SERPIENTE, UN TEMPORIZADOR Y EL SELECCIONADOR DE JUEGO, ADEMÁS DE LA MÚSICA DE FONDO
        private List<Posicion> serpienteJugador1;
        private Posicion direccionJugador1;
        private List<Posicion> serpienteIA;
        private Posicion direccionIA;
        private List<Posicion> serpienteJugador2;
        private Posicion direccionJugador2;
        private Posicion posicionManzana;
        private bool juegoTerminado;
        private const float tamanioSegmentoSerpiente = 20f; //TAMAÑO TOTAL DE CADA CAJITA, OCUPA 20 PIXELES CUADRADOS
        private DispatcherTimer temporizador;
        private SeleccionarModo.ModoJuego modoJuegoActual; //RECOGEMOS EL MODO SELECCIONADO NADA MÁS ABRIR LA VENTANA DEL JUEGO
        private MediaPlayer musicaFondo;

        public MainWindow(SeleccionarModo.ModoJuego modoSeleccionado) //DESDE LA VENTANA DE SELECCION TRAEMOS EL MODO ELEGIDO PARA JUGAR
        {
            InitializeComponent(); //-> CARGARÁ LA INTERFAZ GRÁFICA DEFINIDA EN EL XAML. EL ERROR EN ESTA LÍNEA SE DEBE A FALLO CON EL XAML.
            modoJuegoActual = modoSeleccionado; 

            //UNA VEZ CARGADA TODA LA INTERFAZ Y LOS ELEMENTOS DE MainWindow, EJECUTAMOS EL MÉTODO LOADED
            Loaded += MainWindow_Loaded;
        }

        
        //EL CUÁL INICIALIZA EL JUEGO, CONFIGURA TEMPORIZADORES, CONTROL DE TECLADO, MÚSICA.
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IniciarJuego();
            temporizador = new DispatcherTimer();
            temporizador.Interval = TimeSpan.FromMilliseconds(200); //LA VELOCIDAD DE JUEGO ES 5FPS, FOTO CADA 0,2 SEGUNDOS
            temporizador.Tick += Temporizador_Tick;
            temporizador.Start();

            //DEBEMOS ASEGURAR TENER EL FOCO DEL TECLADO PARA MANEJAR SERPIENTES
            Keyboard.Focus(this); 

            //INSTANCIAMOS MÚSICA, GUARADMOS LA RUTA QUE EN ESTE CASO ESTÁ EN BIN/DEBUG/...
            musicaFondo = new MediaPlayer();
            string rutaMusica = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Musica", "musica_fondo.mp3");
            musicaFondo.Open(new Uri(rutaMusica, UriKind.Absolute));

            musicaFondo.Volume = 0.5; //VOLUMEN AL 50%
            musicaFondo.MediaEnded += MusicaFondo_MediaEnded; //LA MÚSICA SE REPITE AL TERMINAR, DURA UNOS 4 MIN
            musicaFondo.Play();
        }
        private void MusicaFondo_MediaEnded(object sender, EventArgs e)
        {
            //POSITION ES UNA PROPIEDAD DE MEDIAPLAYER QUE ME DICE EN QUÉ SEGUNDO DE REPRODUCCIÓN ESTAMOS, AL INICIO DE LA EJECUCIÓN AL LLAMAR A ESTE MÉTODO
            //HACEMOS QUE EMPIECE EN "0-0-0"
            musicaFondo.Position = TimeSpan.Zero;
            musicaFondo.Play();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            //CONTROLAMOS ERRORES PARANDO EL TEMPORIZADOR SI CERRAMOS VENTANA
            if (temporizador != null && temporizador.IsEnabled)
            {
                temporizador.Stop();
            }

            // TAMBIÉN PARAMOS LA MÚSICA PARA QUE NO SUENE AL CERRAR VENTANA
            if (musicaFondo != null)
            {
                musicaFondo.Stop();
                musicaFondo.Close();
            }
        }

        private void IniciarJuego()
        {
            //SERPIENTE J1 SE INSTANCIA SIEMPRE, DA IGUAL EL MODO DE JUEGO PORQUE SIEMPRE VA A JUGAR
            serpienteJugador1 = new List<Posicion>
            {
                new Posicion(10, 10), //CABEZA
                new Posicion(9, 10),  //CUERPO 1
                new Posicion(8, 10)   //CUERPO 3
            };
            direccionJugador1 = new Posicion(1, 0); //SU DESPLZMIENTO EMPIEZA HACIA LA DERECHA

            juegoTerminado = false;

            //MODO IA
            if (modoJuegoActual == SeleccionarModo.ModoJuego.ContraIA)
            {
                //INICIALIZAMOS LA SERPIENTE DE IA
                serpienteIA = new List<Posicion>
                {
                    //LA COLOCAMOS SEPARADA DE LA DEL JUGADOPR
                    new Posicion(15, 15), //CABEZA
                    new Posicion(16, 15), // CUERPO 1
                    new Posicion(17, 15)  // CUERPO 3
                };
                direccionIA = new Posicion(-1, 0); //ESTA COMENZARÁ DESPLAZÁNDOSE HACIA LA IZQUIERDA A DIF DEL JUGADOR
            }
            else if (modoJuegoActual == SeleccionarModo.ModoJuego.DosJugadores)
            {
                //EN CASO DE 2 JUGADORES, TENEMOS QUE INICIALIZAR LA SERPIENTE DEL J2
                serpienteJugador2 = new List<Posicion>
                {
                    //CUERPO SERP J2
                    new Posicion(15, 15),
                    new Posicion(16, 15), 
                    new Posicion(17, 15)  
                };
                direccionJugador2 = new Posicion(-1, 0); //COMIENZA DESPLZ HACIA IZQ
            }
            else
            {
                //CREAMOS INSTANCIAS VACÍAS SI EL JUGADOR1 QUIERE JUGAR SÓLO
                serpienteIA = new List<Posicion>();
                serpienteJugador2 = new List<Posicion>();
            }

            //LA MANZANA SE GENERARÁ UNA VEZ ESTÁN LAS SERPIENTES, PARA EVITAR QUE SE PINTE SOBRE ALGUNA DE ELLAS
            GenerarNuevaManzana();

            DibujarJuego();
        }

        private void GenerarNuevaManzana()
        {
            Random rand = new Random();
            //VOY A CALCULAR EL NÚMERO MÁXIMO DE POSICIONES LIBRES EN EL EJE X E Y. DENTRO DEL "GameCanvas"
            //DIVIDIMOS ENTRE EL TAMAÑO DEL SEGMENTO DE CADA SERPIENTE. ASÍ VEMOS EL TAMAÑO REAL DE "20F LIBRES"PARA GENERAR UN NÚMERO ENTERO
            int maxX = (int)(GameCanvas.ActualWidth / tamanioSegmentoSerpiente);
            int maxY = (int)(GameCanvas.ActualHeight / tamanioSegmentoSerpiente);
            Posicion nuevaPosicionManzana;

            do
            {
                nuevaPosicionManzana = new Posicion(rand.Next(0, maxX), rand.Next(0, maxY));
            } while (serpienteJugador1.Contains(nuevaPosicionManzana)
            //OBLIGAMOS EN BUCLE A REPETIR LA GENERACIÓN ALEATORIA DE LA MANZANA MIENTRAS ESTA COINCIDA CON LA POSICIÓN DE CUALQUIER SERPIENTE EN JUEGO Y SU TAMAÑO.
            //COMO LAS SERPIENTES SE ALMACENAN EN List, CON .CONTAINS LO COMPROBAMOS.
                     || (modoJuegoActual == SeleccionarModo.ModoJuego.ContraIA && serpienteIA.Contains(nuevaPosicionManzana))
                     || (modoJuegoActual == SeleccionarModo.ModoJuego.DosJugadores && serpienteJugador2.Contains(nuevaPosicionManzana)));

            posicionManzana = nuevaPosicionManzana;
            //LA PINTAMOS
        }

        private void Temporizador_Tick(object sender, EventArgs e)
        {
            //ESTE MÉTODO SE DISPARA CADA VEZ QUE EL TEMPORIZADOR HACE "TICK" QUE EN ESTE CASO ES CADA 0,2 SEGUNDOS
            
            if (!juegoTerminado)
            {
                MoverSerpienteJugador1();
                VerificarColisionesJugador1();
                //MIENTRAS EL JUEGO NO TERMINE, CADA TICK SE MUEVE UNA CASILLA HACIA LA "DIRECCIÓN" QUE EL JUGADOR LE HAYA INDICADO DURANTE EL PERÍODO DEL TICK ANTERIOR
                //A LA VEZ, SE EVALÚAN LAS POSIBLES COLISIONES DE LA SERPIENTE (EN ESTE PRMIER CASO DEL J1). CADA TICK HAY QUE EVALUAR MOVIMIENTO+POSIBLECOLISION
                if (modoJuegoActual == SeleccionarModo.ModoJuego.ContraIA)
                {
                    //LO MISMO QUE J1 PERO SI EL MODO ES CON IA
                    direccionIA = ObtenerDireccionIA();
                    MoverSerpienteIA();
                    VerificarColisionesIA();
                }
                else if (modoJuegoActual == SeleccionarModo.ModoJuego.DosJugadores)
                {
                    //LO MISMO QUE J1 PERO SI EL MODO ES CON J2
                    MoverSerpienteJugador2();
                    VerificarColisionesJugador2();
                }

                DibujarJuego(); //REPINTAMOS LO QUE MOSTRARÁ EL SIGUIENTE TICK CON LOS DATOS OBTENIDOS EN EL ACTUAL.
            }
        }

        private void MoverSerpienteJugador1()
        {
            Posicion nuevaCabeza = serpienteJugador1[0] + direccionJugador1; //ESTE OPERADOR ESTÁ MODIFICADO, SU FUNCIONAMIENTO ESTÁ EN LA CLASE Posicion.cs
            //SI NO TUVIÉSEOMS ESE MÉTODO OPERADOR + EN LA CLASE Posicion, TENDRÍAMOS QUE HACER ESTO:
            //Posicion nuevaCabeza = new Posicion(serpienteJugador1[0].X + direccionJugador1.X, serpienteJugador1[0].Y + direccionJugador1.Y);

            serpienteJugador1.Insert(0, nuevaCabeza);

            //CADA TICK VERIFICAMOS SI AL MOVER LA SERPIENTE HEMOS PASADO POR LA POSICIÓN DONDE SE ENCONTRABA YA PINTADA LA MANZANA
            if (nuevaCabeza == posicionManzana)
            {
                GenerarNuevaManzana(); //SI COMIDA, GENERAMOS MANZANA Y NO ELIMINAMOS EL ÚLTIMO CUADRADITO DEL CUERPO QUE SE ENCONTRABA ENE L TICK ANTERIOR
            }
            else
            {
                serpienteJugador1.RemoveAt(serpienteJugador1.Count - 1); //EN CASO CONTRARIO, ELIMINAMOS 1 CUADRADITO DEL CUERPO
            }
        }

        private void VerificarColisionesJugador1()
        {
            //ESTO ALGO MÁS AVANZADILLO, LO PRIMERO ES RECOGER EL SEGMENTO DE LA CABEZA DE LA SERPIENTE (POSICIÓMN 0)
            //CALCULAR EL TAMAÑO RESTANTE DE CUADROS LIBRES (CADA UNO DE 20X20 PIXELES) DONDE LA SERPIENTE PUEDE ESTAR
            
            Posicion cabeza = serpienteJugador1[0];
            int maxX = (int)(GameCanvas.ActualWidth / tamanioSegmentoSerpiente);
            int maxY = (int)(GameCanvas.ActualHeight / tamanioSegmentoSerpiente);

            //SI SE INTENTA SALIR DE CUALQUIERA DE LOS LÍMITES DE LA PANTALLA:
            //LA CABEZA SALE DEL EJE X E Y -> PIERDE
            //X=0 E Y=0 ES ARRIBA IZQUIERDA DEL TODO. MAxX Y MAXY ABAJO Y DERECHA DEL TODO
            if (cabeza.X < 0 || cabeza.X >= maxX || cabeza.Y < 0 || cabeza.Y >= maxY)
            {
                FinDelJuego("¡Te has chocado contra el borde! :(", 1); //LE DAMOS TRUE AL BOOL DEL CONSTTRCTOR
            }

            // RECORREMOS LA "List" DE LA SERPIENTE, SI LA CABEZA COINCIDE CON CUALQUIERA DE SUS SEGMENTOS, TAMBIÉN PIERDE
            for (int i = 1; i < serpienteJugador1.Count; i++)
            {
                if (cabeza == serpienteJugador1[i])//AQUÍ SÍ QUE USAMOS EL OPERADOR MODIFICADO ==
                {
                    FinDelJuego("OOHHH NO!! Te has chocado contra el borde... ^^", 1);
                    break;
                }
            }

            if (modoJuegoActual == SeleccionarModo.ModoJuego.ContraIA)
            {
                //PARA IA Y J2 COMPRUEBO CON FOREACH
                if (serpienteIA.Any(segmento => segmento == cabeza)) //AQUÍ SÍ QUE USAMOS EL OPERADOR MODIFICADO ==
                {
                    FinDelJuego("¡Te has chocado contra la IA! jeje", 1);
                }
            }
            else if (modoJuegoActual == SeleccionarModo.ModoJuego.DosJugadores)
            {
                //J2
                if (serpienteJugador2.Any(segmento => segmento == cabeza))
                {
                    FinDelJuego("Puff... Estampado contra el Jugador 2!!", 1);
                }
            }
        }


        //MÉTODO MOVIMIENTO IA
        private void MoverSerpienteIA()
        {
            Posicion nuevaCabeza = serpienteIA[0] + direccionIA;
            serpienteIA.Insert(0, nuevaCabeza);
            if (nuevaCabeza == posicionManzana)
            {
                GenerarNuevaManzana(); 
            }
            else
            {
                serpienteIA.RemoveAt(serpienteIA.Count - 1); //ÚLTIMA POSICIÓN ELIMINADA
            }
        }

        private void VerificarColisionesIA()
        {
            Posicion cabeza = serpienteIA[0];
            int maxX = (int)(GameCanvas.ActualWidth / tamanioSegmentoSerpiente);
            int maxY = (int)(GameCanvas.ActualHeight / tamanioSegmentoSerpiente);
            if (cabeza.X < 0 || cabeza.X >= maxX || cabeza.Y < 0 || cabeza.Y >= maxY)
            {
                FinDelJuego("¡ENHORABUENA! La serpiente IA se ha estampado contra el borde (Y eso es muy difícil de lograr ^^)", 2);
            }
            for (int i = 1; i < serpienteIA.Count; i++)
            {
                if (cabeza == serpienteIA[i])
                {
                    FinDelJuego("La serpiente IA se ha chocado consigo misma", 2); //ESTO NO VA A OCURRIR CON EL ALGORITMO A* A NO SER QUE NO TENGA ESPACIOS, PERO BUENO
                    break;
                }
            }
            if (serpienteJugador1.Any(segmento => segmento == cabeza))
            {
                FinDelJuego("La serpiente IA ha chocado contigo.", 2);
            }
        }

        private void MoverSerpienteJugador2()
        {
            Posicion nuevaCabeza = serpienteJugador2[0] + direccionJugador2;
            serpienteJugador2.Insert(0, nuevaCabeza);
            if (nuevaCabeza == posicionManzana)
            {
                GenerarNuevaManzana();
            }
            else
            {
                serpienteJugador2.RemoveAt(serpienteJugador2.Count - 1);
            }
        }

        private void VerificarColisionesJugador2()
        {
            Posicion cabeza = serpienteJugador2[0];
            int maxX = (int)(GameCanvas.ActualWidth / tamanioSegmentoSerpiente);
            int maxY = (int)(GameCanvas.ActualHeight / tamanioSegmentoSerpiente);
            if (cabeza.X < 0 || cabeza.X >= maxX || cabeza.Y < 0 || cabeza.Y >= maxY)
            {
                FinDelJuego("Se ha chocado contra el borde.", 2);
            }
            for (int i = 1; i < serpienteJugador2.Count; i++)
            {
                if (cabeza == serpienteJugador2[i])
                {
                    FinDelJuego("Se ha chocado consigo mismo.", 2);
                    break;
                }
            }
            if (serpienteJugador1.Any(segmento => segmento == cabeza))
            {
                FinDelJuego("Ha chocado con el Jugador 1.", 2);
            }
        }

        private Posicion ObtenerDireccionIA()
        {
            //VOY A UTILIZAR EL ALGORITMO A* PARA QUE LA IA RECALCULE LA RUTA MÁS ÓPTIMA EN CADA 
            //TENEMOS QUE VER LOS CUADROS ABIERTOS Y CERRADOS, INSTANCIAMOS EN VACÍO

            //PERO QUÉ HACE ESTE ALGORITMO? EXPLORA LAS RUTAS POSIBLES DESDE
            //UN NODO INICIAL HASTA UN NODO OBJETIVO (O META), DURANTE EL PROCESO, ES DECIR
            //CADA TICK, VA A CALCULAR EL CAMINO POTENCIALMENTE MÁS CORTO
            //PARA ESTO A* UTILIZA UNA ´"FUNCIÓN DE EVALUACIÓN DE COSTOS" QUE CONSIDERA DOS FACTORES PARA DECIDIR
            //EL MEJOR CAMINO

            //"g" REPRESENTA EL COSTO REAL HASTA EL MOMENTO (DISTANCIA ENTRE NODO ORIGEN Y FINAL)
            //"h" ES EL COSTO ESTIMADO HASTA EL OBJETIVO, ES DECIR, DISTANCIA DEL NODO ACTUAL HASTA EL OBJETIVO CALCULADA
            //COMO LA DISTANCIA "MANHATTAN" O "EUCLIDIANA"

            //LA FÓRMULA ES SENCILLA: f(n)=g(n)+h(n) SE AGREGAN AL NODO INICIAL Y SU f(n) A UNA LISTA
            //DE NODOS ABIERTOS QUE AÚN DEBEN RECORRERSE/EXLPORARSE/EVALUARSE
            //SE SELECCIONA EL NODO CON EL VALOR f(n) MÁS BAJO DE LA LISTA DE NODOS ABIERTOS
            //SI EL NODO ACTUAL ES EL OBJETIVO, EL ALGORITMO SE DETIENE, HABRÁ ENCONTRADO EL CAMINO MÁS CORTO

            //SI NO, ELIMINAMOS EL NODO DE ABIERTOS Y LO METEMOS EN CERRADOS (YA EVALUADOS)

            //LA ÚNICA PEGA ES QUE CONSUME MUCHOS RECURSOS. Y QUE LA IA ES UN OPONENTE VERDADERAMENTE DIFÍCIL DE GANAR

            List<Nodo> abiertos = new List<Nodo>();
            List<Nodo> cerrados = new List<Nodo>(); //CON ESTA LISTA EVITAMOS RE-EXPLORAR EL MISMO CAMINO VARIAS VECES

            // NODO PUNTO DE PARTIDA, QUE ES LA CABEZA DE LA IA, EL NODO PADRE AHORA ES NULL, PORQUE ES EL PUNTO INICIAL
            //3er PARÁMETRO ES 0 PORQUE DESDE LA CABEZA HASTA LA CABEZA HAY 0 POSICIONES DE DISTANCIA (COSTO REAL "g")

            //EL MÉTODO CalcularHeuristica CALCULA UNA ESTIMACIÓN DE LA DISTANCIA ENTRE LA CABEZA Y LA MANZANA, LO VEMOS DESPUÉS

            //EN RESUMEN, ESTABLEZCO EL ORIGEN Y UNA ESTIMACIÓN DEL CAMINO HACIA LA MANZANITA
            Nodo nodoInicial = new Nodo(serpienteIA[0], null, 0, CalcularHeuristica(serpienteIA[0], posicionManzana));
            abiertos.Add(nodoInicial);
            //AÑADO EL NODO INICIAL A LA LISTA DE EVALUABLES

            //RECORRO TODOS LOS NODOS ABIERTOS
            while (abiertos.Count > 0)
            {
                //QUÉ ES ESTO?: SELECCIONA EL NODO CON EL VALOR F MÁS BAJO EN ABIERTOS. f=g+h.
                Nodo nodoActual = abiertos.OrderBy(n => n.F).First();
                //ORDENADO EL CAMINO MÁS EFICIENTE HAST AEL MOMENTO
                if (nodoActual.Posicion == posicionManzana)
                {
                    //EN CADA TICK, SI NO ESTAMOS EN LA MANZANA, RE-EVALUAMOS Y DE SER NECESARIO, RE-DIRIGIMOS LA SERPIENTE IA
                    return ReconstruirDireccion(nodoActual);
                }

                abiertos.Remove(nodoActual);
                cerrados.Add(nodoActual);

                foreach (var vecino in ObtenerVecinos(nodoActual))
                {
                    //RECORREMOS LOS VECINOS DEL NODO ACTUAL, SI ESTÁN CERRADOS SE IGNORAN
                    if (cerrados.Any(n => n.Posicion == vecino.Posicion))
                        continue;

                    //SI EL VECINO NO ESTÁ EN CERRADOS NI EN ABIERTOS LO AÑADIMOS A ABIERTOS
                    var nodoAbierto = abiertos.FirstOrDefault(n => n.Posicion == vecino.Posicion);
                    if (nodoAbierto == null)
                    {
                        abiertos.Add(vecino);
                    }
                    else
                    {
                        //EL CAMINO VECINO ES MEJOR QUE EL ACTUAL? LO CAMBIAMOS
                        if (vecino.G < nodoAbierto.G)
                        {
                            nodoAbierto.G = vecino.G;
                            nodoAbierto.Padre = nodoActual;
                        }
                    }
                }
            }

            // SI NO ENCONTRAMOS CAMINO, MANTENEMOS DIRECCIÓN
            return direccionIA;
        }

        private int CalcularHeuristica(Posicion desde, Posicion hasta)
        {
            // DISTANCIA DE MANHATTAN. "DESDE" REPRESENTA LA POSICIÓN ACTUAL DE LA CABEZA IA, HASTA REPRESENTA LA POSICIÓN OBJETIVO
            //ES DECIR, CALCULAMOS LA DISTANCIA HORIZONTAL (X) Y LA VERTICAL(Y) HASTA LA MANZANA 
            
            //EJEMPLO SIMPLE: SI LA POSICIÓN ACTUAL DE LA SERPIENTE ES (2,3) Y LA MANZANA ESTÁ EN (5,1)
            //(2-5) + (3-1) = 3+2 = 5 , PORQUE DE LA 2 A LA 5 HAY 3 POSICIIONES Y DE LA 3 A LA 1 HAY 2. ES DECIR 5
            //COMO ES DISTANCIA, LOS VALORES NEGATIVOS NO SE TIENEN EN CUENTA, SOLAMENTE INDICARÍA HACIA ABAJO O IZQUIERDA
            //Y LOS POSITIVOS HACIA ARRIBA O DERECHA
            return Math.Abs(desde.X - hasta.X) + Math.Abs(desde.Y - hasta.Y);
        }

        private List<Nodo> ObtenerVecinos(Nodo nodoActual)
        {
            List<Nodo> vecinos = new List<Nodo>();
            List<Posicion> direcciones = new List<Posicion>
            {
                //SOLAMENTE VAMOS A TENER 4 VECINOS, DEBEMOS RECORRER CADA DIRECCIÓN Y EVALUAR SI ES POSICIÓN VÁLIDA PARA IR Y EVALUAR SI PERMITE UNA RUTA MÁS CORTA
                new Posicion(0, -1), // ARRIBA
                new Posicion(0, 1),  // ABAJO
                new Posicion(-1, 0), // IZQ
                new Posicion(1, 0)   // DERECHA
            };

            foreach (var dir in direcciones)
            {
                Posicion nuevaPosicion = nodoActual.Posicion + dir;

                if (EsPosicionValida(nuevaPosicion))
                {
                    //EL VECINO, NO USA NULL PORQUE NO ES EL PADRE Y EN LUGAR DE 0 EL TERCER PARÁMETRO ES G+1 QUE LLEVA ACUMULANDO LOS COSTES DE LA RUTA
                    Nodo vecino = new Nodo(nuevaPosicion, nodoActual, nodoActual.G + 1, CalcularHeuristica(nuevaPosicion, posicionManzana));
                    vecinos.Add(vecino);
                }
            }

            return vecinos;
        }

        private bool EsPosicionValida(Posicion posicion)
        {
            int maxX = (int)(GameCanvas.ActualWidth / tamanioSegmentoSerpiente);
            int maxY = (int)(GameCanvas.ActualHeight / tamanioSegmentoSerpiente);

            //MÉTODO NORMAL DONDE NO VOY A USAR EL OPERADOR MODIFICADO DE Posicion
            if (posicion.X < 0 || posicion.X >= maxX || posicion.Y < 0 || posicion.Y >= maxY)
                return false;

            if (serpienteJugador1.Any(segmento => segmento == posicion))
                return false;

            if (modoJuegoActual == SeleccionarModo.ModoJuego.DosJugadores)
            {

                if (serpienteJugador2.Any(segmento => segmento == posicion))
                    return false;
            }

            var cuerpoIA = serpienteIA.Take(serpienteIA.Count - 1);
            if (cuerpoIA.Any(segmento => segmento == posicion))
                return false;

            return true;
        }

        private Posicion ReconstruirDireccion(Nodo nodoObjetivo)
        {
            Nodo nodo = nodoObjetivo;

            // Retroceder hasta el nodo hijo directo de la cabeza
            while (nodo.Padre != null && nodo.Padre.Posicion != serpienteIA[0])
            {
                nodo = nodo.Padre;
            }

            // DIRECCION=NODO-CABEZA
            int deltaX = nodo.Posicion.X - serpienteIA[0].X;
            int deltaY = nodo.Posicion.Y - serpienteIA[0].Y;

            // NORMALIZAMOS DIR
            if (deltaX != 0) deltaX = deltaX / Math.Abs(deltaX);
            if (deltaY != 0) deltaY = deltaY / Math.Abs(deltaY);

            return new Posicion(deltaX, deltaY);
        }

        private void FinDelJuego(string mensaje, int jugadorPerdedor)
        {
            juegoTerminado = true;
            temporizador.Stop();

            string mensajeFinal = mensaje;

            if (modoJuegoActual == SeleccionarModo.ModoJuego.DosJugadores)
            {
                int jugadorGanador = jugadorPerdedor == 1 ? 2 : 1;
                mensajeFinal = $"¡El Jugador {jugadorPerdedor} ha perdido! {mensaje}\n\n" +
                               $"¡El Jugador {jugadorGanador} ha ganado!";
            }
            else if (modoJuegoActual == SeleccionarModo.ModoJuego.ContraIA)
            {
                if (jugadorPerdedor == 1)
                {
                    mensajeFinal = $"¡Has perdido! {mensaje}";
                }
                else
                {
                    mensajeFinal = $"¡Has ganado! {mensaje}";
                }
            }
            else //UN JUGADOR
            {
                mensajeFinal = $"¡Has perdido! :( {mensaje}";
            }

            var resultado = MessageBox.Show($"{mensajeFinal}\n\n¿Quieres jugar de nuevo?", "Fin del juego", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (resultado == MessageBoxResult.Yes)
            {
                // CADA FIN DE PARTIDA EN SI ABRIMOS LOS MODOS DE JUEGO
                SeleccionarModo seleccionarModoDialogo = new SeleccionarModo();
                bool? resultadoSeleccion = seleccionarModoDialogo.ShowDialog();

                if (resultadoSeleccion == true)
                {
                    modoJuegoActual = seleccionarModoDialogo.ModoSeleccionado;
                    IniciarJuego();
                    temporizador.Start();
                }
                else
                {
                    //SI CERRAMOS VENTANA O CANCELAMOS, FUERA
                    Close();
                }
            }
            else
            {
                Close();
            }
        }


        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //FLECHAS PARA MOVER JUGADOR 1
            switch (e.Key)
            {
                case Key.Left:
                    //EN TODOS LOS MOVIMIENTOS EVITAMOS QUE, SI VAMOS HACIA UNA DIRECCION, PODAMOS IR HACIA LA CONTRARIA
                    if (direccionJugador1 != new Posicion(1, 0)) direccionJugador1 = new Posicion(-1, 0);
                    break;
                case Key.Right:
                    if (direccionJugador1 != new Posicion(-1, 0)) direccionJugador1 = new Posicion(1, 0);
                    break;
                case Key.Up:
                    if (direccionJugador1 != new Posicion(0, 1)) direccionJugador1 = new Posicion(0, -1);
                    break;
                case Key.Down:
                    if (direccionJugador1 != new Posicion(0, -1)) direccionJugador1 = new Posicion(0, 1);
                    break;
            }

            if (modoJuegoActual == SeleccionarModo.ModoJuego.DosJugadores)
            {
                // AWSD JUGADOR 2
                switch (e.Key)
                {
                    case Key.A:
                        if (direccionJugador2 != new Posicion(1, 0)) direccionJugador2 = new Posicion(-1, 0);
                        break;
                    case Key.D:
                        if (direccionJugador2 != new Posicion(-1, 0)) direccionJugador2 = new Posicion(1, 0);
                        break;
                    case Key.W:
                        if (direccionJugador2 != new Posicion(0, 1)) direccionJugador2 = new Posicion(0, -1);
                        break;
                    case Key.S:
                        if (direccionJugador2 != new Posicion(0, -1)) direccionJugador2 = new Posicion(0, 1);
                        break;
                }
            }
        }

        private void DibujarJuego()
        {
            //LIMPIAMOS EL CANVAS
            GameCanvas.Children.Clear();

            // DIBUJAMOS J1
            foreach (var segmento in serpienteJugador1)
            {
                Rectangle rectangulo = new Rectangle
                {
                    Width = tamanioSegmentoSerpiente,
                    Height = tamanioSegmentoSerpiente,
                    Fill = Brushes.Red
                };
                Canvas.SetLeft(rectangulo, segmento.X * tamanioSegmentoSerpiente);
                Canvas.SetTop(rectangulo, segmento.Y * tamanioSegmentoSerpiente);
                GameCanvas.Children.Add(rectangulo);
            }

            if (modoJuegoActual == SeleccionarModo.ModoJuego.ContraIA)
            {
                // DIBUJAMOS SERPIENTE IA
                foreach (var segmento in serpienteIA)
                {
                    Rectangle rectangulo = new Rectangle
                    {
                        Width = tamanioSegmentoSerpiente,
                        Height = tamanioSegmentoSerpiente,
                        Fill = Brushes.Purple
                    };
                    Canvas.SetLeft(rectangulo, segmento.X * tamanioSegmentoSerpiente);
                    Canvas.SetTop(rectangulo, segmento.Y * tamanioSegmentoSerpiente);
                    GameCanvas.Children.Add(rectangulo);
                }
            }
            else if (modoJuegoActual == SeleccionarModo.ModoJuego.DosJugadores)
            {
                // DIBUJAMOS J2
                foreach (var segmento in serpienteJugador2)
                {
                    Rectangle rectangulo = new Rectangle
                    {
                        Width = tamanioSegmentoSerpiente,
                        Height = tamanioSegmentoSerpiente,
                        Fill = Brushes.Blue
                    };
                    Canvas.SetLeft(rectangulo, segmento.X * tamanioSegmentoSerpiente);
                    Canvas.SetTop(rectangulo, segmento.Y * tamanioSegmentoSerpiente);
                    GameCanvas.Children.Add(rectangulo);
                }
            }

            // DIBUJAMOS MANZANA
            Rectangle manzana = new Rectangle
            {
                Width = tamanioSegmentoSerpiente,
                Height = tamanioSegmentoSerpiente,
                Fill = Brushes.Green
            };
            Canvas.SetLeft(manzana, posicionManzana.X * tamanioSegmentoSerpiente);
            Canvas.SetTop(manzana, posicionManzana.Y * tamanioSegmentoSerpiente);
            GameCanvas.Children.Add(manzana);
        }
    }
}
