using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd_LID_GAMES.Entidades
{
    public class Puntaje
    {

        public int idJuego { get; set; }

        public int idUsuario { get; set; }

        public string nombreUsuario { get; set; }

        public int puntos { get; set; }

        public string posicion { get; set; }
    }
}
