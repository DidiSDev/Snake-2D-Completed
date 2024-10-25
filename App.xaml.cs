//App.xaml.cs
using System.Windows;

namespace Snake2D
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            
            // SOLUCIONAMOS EL ERROR QUE SE PRODUCÍA AL ELEGIR MODO. EVITAMOS QUE LA APLICACIÓN SE CIERRE AL CERRARSE LA VENTANA MODO
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            SeleccionarModo seleccionarModoDialogo = new SeleccionarModo();
            bool? resultado = seleccionarModoDialogo.ShowDialog();

            if (resultado == true)
            {
                //CREAMOS Y MOSTRAMOS LA VENTANA PRINCIPAL DEL JUEGO, TRAS EL RESULTADO -> TRUE
                MainWindow mainWindow = new MainWindow(seleccionarModoDialogo.ModoSeleccionado);
                this.MainWindow = mainWindow;
                //AHORA LA APP SE CERRARÁ TRAS CERRAR LA VENTANA PRINCIPAL
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                mainWindow.Show();
            }
            else
            {
                //SI CERRAMOS MODO O CANCELAMOS, SHUTDOWN
                this.Shutdown();
            }
        }
    }
}
