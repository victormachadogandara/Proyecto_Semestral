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


namespace CambioImagenes
{
    public class Pacman
    {
        List<BitmapImage> arriba = new List<BitmapImage>();
        List<BitmapImage> abajo = new List<BitmapImage>();
        List<BitmapImage> izquierda = new List<BitmapImage>();
        List<BitmapImage> derecha = new List<BitmapImage>();
        List<BitmapImage> estatico = new List<BitmapImage>();

        Image Imagen { get; set; }

        public enum Direccion { Izquierda, Derecha, Arriba, Abajo, Estatico};
        Direccion DireccionActual { get; set; }

        public double PosicionX { get; set; }
        public double PosicionY { get; set; }

        public double Velocidad { get; set; }

        int spriteActual = 0;
        double tiempoTranscurridoEnSprite = 0;
        double tiempoPorSprite = 0.25;

        double tiempo = 0;

        public Pacman(Image imagen)
        {
            Imagen = imagen;
            //arriba.Add(new BitmapImage(new Uri("Zero_jump.png", UriKind.Relative)));
            arriba.Add(new BitmapImage(new Uri("Zero_jump.png", UriKind.Relative)));
            arriba.Add(new BitmapImage(new Uri("Zero_jump.png", UriKind.Relative)));

            //abajo.Add(new BitmapImage(new Uri("Zero_down.png", UriKind.Relative)));
            abajo.Add(new BitmapImage(new Uri("Zero_down.png", UriKind.Relative)));
            abajo.Add(new BitmapImage(new Uri("Zero_down.png", UriKind.Relative)));

            izquierda.Add(new BitmapImage(new Uri("Zero_run_1.png", UriKind.Relative)));
           // izquierda.Add(new BitmapImage(new Uri("Zero_run_2.png", UriKind.Relative)));
            izquierda.Add(new BitmapImage(new Uri("Zero_run_3.png", UriKind.Relative)));

            derecha.Add(new BitmapImage(new Uri("Zero_run_1.png", UriKind.Relative)));
           // derecha.Add(new BitmapImage(new Uri("Zero_run_2.png", UriKind.Relative)));
            derecha.Add(new BitmapImage(new Uri("Zero_run_3.png", UriKind.Relative)));

            estatico.Add(new BitmapImage(new Uri("Zero_down.png", UriKind.Relative)));
            estatico.Add(new BitmapImage(new Uri("Zero_down.png", UriKind.Relative)));

            Imagen.Source = derecha[0];

            PosicionX = Canvas.GetLeft(imagen);
            PosicionY = Canvas.GetTop(imagen);

            DireccionActual = Direccion.Derecha;

            Velocidad = 0;

        }

        public void CambiarDireccion(Direccion nuevaDireccion)
        {
            DireccionActual = nuevaDireccion;
            switch (DireccionActual)
            {
                case Direccion.Abajo:
                    Imagen.Source = abajo[0];
                    break;
                case Direccion.Arriba:
                    Imagen.Source = arriba[0];
                    break;
                case Direccion.Izquierda:
                    Imagen.Source = izquierda[0];
                    break;
                case Direccion.Derecha:
                    Imagen.Source = derecha[0];
                    break;
                case Direccion.Estatico:
                    Imagen.Source = estatico[0];
                    break;
                default:
                    break;
            }
        }

        public void Mover(double deltaTime)
        {
            
            tiempoTranscurridoEnSprite += deltaTime;
            int spriteAnterior = spriteActual;
            if (tiempoTranscurridoEnSprite >= tiempoPorSprite)
            {
                spriteActual++;
                tiempoTranscurridoEnSprite -= tiempoPorSprite;
                if (spriteActual > 1)
                {
                    spriteActual = 0;
                }
            }
            BitmapImage sprite = null;
            switch (DireccionActual)
            {
                case Direccion.Abajo:
                    PosicionY -= Velocidad * deltaTime;
                    sprite = abajo[spriteActual];
                    Velocidad = -200;
         
                    break;
                case Direccion.Arriba:
             
                        for(double i = 0; i < 1; i+= deltaTime)
                    {
                        Velocidad = 0.2;
                        PosicionY -= Velocidad * deltaTime;
                        sprite = arriba[spriteActual];
                    }
              
                    break;
                case Direccion.Izquierda:
                    PosicionX -= Velocidad * deltaTime;
                    sprite = izquierda[spriteActual];
                    Velocidad = 0;
                    break;
                case Direccion.Derecha:
                    PosicionX += Velocidad * deltaTime;
                    sprite = derecha[spriteActual];
                    Velocidad = 0;
                    break;
                case Direccion.Estatico:
                    //PosicionY += Velocidad * deltaTime;
                    sprite = estatico[spriteActual];
                    Velocidad = 0;
                    break;
                default:
                    Velocidad = 0;
                    break;
            }
            if (spriteAnterior != spriteActual && sprite != null)
            {
                Imagen.Source = sprite;
            }
            Canvas.SetLeft(Imagen, PosicionX);
            Canvas.SetTop(Imagen, PosicionY);


        }
    }
}
