using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LID_Games_Arcade.Entidades
{
    public class EnemigoAlien
    {
        public int vidaEnemigo {  get; set; }

        public bool estadoVida { get; set; }

        public string tipoEnemigo { get; set; }

        public double velocidadDisparo {  get; set; }

        public double puntosQueVale { get; set; }    
    }
}
