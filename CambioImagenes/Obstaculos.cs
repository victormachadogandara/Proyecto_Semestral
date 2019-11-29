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
    class Obstaculos
    {
        List<BitmapImage> izquierda = new List<BitmapImage>();

        Image Imagen { get; set; }

        double PosicionX { get; set; }
        double Posicion1 { get; set; }

        public double Velocidad { get; set; }

        int spriteActual = 0;
        double tiempoTranscurridoEnSprite = 0;
        double tiempoPorSprite = 0.25;

        public Obstaculos(Image imagen)
        {
            Imagen = imagen;

            izquierda.Add(new BitmapImage(new Uri("doge.png", UriKind.Relative)));

            Imagen.Source = izquierda[0];

            PosicionX = Canvas.GetLeft(imagen);

            Velocidad = 40;
        }

        public void Mover(double deltaTime,Image imagen)
        {
            Posicion1 = Canvas.GetLeft(imagen);
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
            
            PosicionX -= Velocidad * deltaTime;
            //sprite = izquierda[spriteActual];
            
                    
               
            if (spriteAnterior != spriteActual && sprite != null)
            {
                Imagen.Source = sprite;
            }

            if(Posicion1 <= 10)
            {
                PosicionX = 806;
            }
            Canvas.SetLeft(Imagen, PosicionX);
        }
    }
}
