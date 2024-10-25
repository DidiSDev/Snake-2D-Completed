using System.Windows;
using System.Windows.Input;

namespace Snake2D
{
    public partial class SeleccionarModo : Window
    {
        //ENUMERACIÓN
        public enum ModoJuego
        {
            UnJugador,
            DosJugadores,
            ContraIA
        }

        public ModoJuego ModoSeleccionado { get; private set; } //ESPECIFICAMOS QUE SOLAMENTE ESTA CLASE PUEDA CAMBIAR EL MODO SELECCIONADO, PARA EVITAR CONFLICTOS 
        //O POSIBLES ERRORES AL ESCRIBIR EN EL .CS DE EJECUCIÓN DEL JUEGO 

        public SeleccionarModo()
        {
            InitializeComponent();
        }
        private void BotonSalir_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void BotonUnJugador_Click(object sender, RoutedEventArgs e)
        {
            ModoSeleccionado = ModoJuego.UnJugador;
            DialogResult = true;
        }

        private void BotonDosJugadores_Click(object sender, RoutedEventArgs e)
        {
            ModoSeleccionado = ModoJuego.DosJugadores;
            DialogResult = true;
        }

        private void BotonContraIA_Click(object sender, RoutedEventArgs e)
        {
            ModoSeleccionado = ModoJuego.ContraIA;
            DialogResult = true;
        }

        private void BotonCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
