using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

using NAudio;
using NAudio.Wave;
using NAudio.Dsp;

namespace CambioImagenes
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaveIn waveIn; //Conexion con microfono
        WaveFormat formato; //Formato de audio

        bool jugando = true;
        Pacman pacman;
        Obstaculos obstaculo;
        Stopwatch stopwatch = new Stopwatch();
        TimeSpan tiempoAnterior;
        double tiempo = 0;

       
        
        public MainWindow()
        {
            //Inicializar la conexion
            waveIn = new WaveIn();

            //Establecer el formato
            waveIn.WaveFormat =
                new WaveFormat(44100, 16, 1);
            formato = waveIn.WaveFormat;

            //Duracion del buffer
            waveIn.BufferMilliseconds = 500;

            //Con que funcion respondemos
            //cuando se llena el buffer
            waveIn.DataAvailable += WaveIn_DataAvailable;

            waveIn.StartRecording();

            InitializeComponent();

            canvasPrincipal.Focus();

            pacman = new Pacman(spritePacman);
            obstaculo = new Obstaculos(spriteObstaculo);

            stopwatch.Start();
            tiempoAnterior = stopwatch.Elapsed;


            ThreadStart threadStart = new ThreadStart(cicloPrincipal);
            Thread thread = new Thread(threadStart);
            thread.Start();

        }

        private void WaveIn_DataAvailable(object sender,
            WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesGrabados = e.BytesRecorded;

            int numMuestras = bytesGrabados / 2;

            int exponente = 0;
            int numeroBits = 0;

            do
            {
                exponente++;
                numeroBits = (int)
                    Math.Pow(2, exponente);
            } while (numeroBits < numMuestras);
            exponente -= 1;
            numeroBits = (int)
                Math.Pow(2, exponente);
            Complex[] muestrasComplejas =
                new Complex[numeroBits];

            for (int i = 0; i < bytesGrabados; i += 2)
            {
                short muestra =
                    (short)(buffer[i + 1] << 8 | buffer[i]);
                float muestra32bits =
                    (float)muestra / 32768.0f;
                if (i / 2 < numeroBits)
                {
                    muestrasComplejas[i / 2].X = muestra32bits;
                }

            }

            FastFourierTransform.FFT(true,
                exponente, muestrasComplejas);

            float[] valoresAbsolutos =
                new float[muestrasComplejas.Length];

            for (int i = 0; i < muestrasComplejas.Length;
                i++)
            {
                valoresAbsolutos[i] = (float)
                    Math.Sqrt(
                    (muestrasComplejas[i].X * muestrasComplejas[i].X) +
                    (muestrasComplejas[i].Y * muestrasComplejas[i].Y));

            }

            var mitadValoresAbsolutos =
                valoresAbsolutos.Take(valoresAbsolutos.Length / 2).ToList();

            int indiceValorMaximo =
                mitadValoresAbsolutos.IndexOf(
                mitadValoresAbsolutos.Max());

            float frecuenciaFundamental =
               (float)(indiceValorMaximo * formato.SampleRate)
               / (float)valoresAbsolutos.Length;

            if (frecuenciaFundamental >= 1000)
            {
                pacman.CambiarDireccion(Pacman.Direccion.Arriba);
            }

            lblHertz.Text =
                frecuenciaFundamental.ToString("n") +
                " Hz";

        }
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            canvasReglas.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            canvasReglas.Visibility = Visibility.Collapsed;
            canvasPrincipal.Visibility = Visibility.Visible;
            stopwatch.Start();
            canvasPrincipal.Focus();
        }

        public void cicloPrincipal()
        {
            while (jugando)
            {
                Dispatcher.Invoke(actualizar);
            }
        }

        public void actualizar()
        {

            //while (true)
            //{
            //    Dispatcher.Invoke(
            //    () =>
            //    {
            
            TimeSpan tiempoActual = stopwatch.Elapsed;
            double deltaTime = tiempoActual.TotalSeconds - tiempoAnterior.TotalSeconds;
            lblScore.Text = (stopwatch.Elapsed.Minutes.ToString())+":"+(stopwatch.Elapsed.Seconds.ToString());
            pacman.Mover(deltaTime);
            pacman.Velocidad += 10 * deltaTime;




            obstaculo.Mover(deltaTime, spriteObstaculo);
            obstaculo.Velocidad += 10 * deltaTime;
            if (pacman.PosicionY <= 68)
            {
                pacman.CambiarDireccion(Pacman.Direccion.Abajo);
            }
            tiempoAnterior = tiempoActual;

            if (pacman.PosicionY >= 263)
            {
                pacman.CambiarDireccion(Pacman.Direccion.Estatico);
            }
            
            double leftpersonajeActual = Canvas.GetLeft(spritePacman);
            Canvas.SetLeft(spritePacman, leftpersonajeActual - (20 * deltaTime));

            if (Canvas.GetLeft(spritePacman) <= -100)
            {
                Canvas.SetLeft(spritePacman, 800);
            }


            //Intersección en X
            double xPersonaje =
                Canvas.GetLeft(spritePacman);
            double xObstaculo =
                Canvas.GetLeft(spriteObstaculo);

            double yPersonaje =
                Canvas.GetTop(spritePacman);
            double yObstaculo =
                Canvas.GetTop(spriteObstaculo);

            if (xObstaculo + spriteObstaculo.Width >= xPersonaje &&
                xObstaculo <= xPersonaje + spritePacman.Width &&
                yObstaculo + spriteObstaculo.Height >= yPersonaje &&
                yObstaculo <= yPersonaje + spritePacman.Height)
            {
                lblColision.Text =
                    "Game Over";
                pacman.Velocidad = 0;
                obstaculo.Velocidad = 0;
                stopwatch.Stop();

            }
            else
            {
                lblColision.Text =
                    "";
            }








            //        tiempoAnterior = tiempoActual;


            //    }
            //    );
            //}






        }

        private void Window_Closed(object sender, EventArgs e)
        {
            jugando = false;
        }

        private void canvasPrincipal_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                if (e.Key == Key.Space)
                {
                    pacman.CambiarDireccion(Pacman.Direccion.Arriba);
                }
                if (e.Key == Key.None)
                {
                    pacman.CambiarDireccion(Pacman.Direccion.Abajo);
                }           
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
