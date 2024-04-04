using LID_Games_Arcade;

namespace FrontEnd_LID_GAMES;

public partial class MenuPausaWorm : ContentPage
{
	public MenuPausaWorm()
	{
		InitializeComponent();
	}

    private void playResumeWormbtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        WormJuego.tiempoManzana.Start();
        WormJuego.tiempoWorm.Start();
        Navigation.PopAsync(); 
    }

    private void botonPresionado(object sender, EventArgs e)
    {
        ImageButton imagen = (ImageButton)sender;
        imagen.Scale = 0.8;
    }

    private void botonLiberado(object sender, EventArgs e)
    {
        ImageButton imagen = (ImageButton)sender;
        imagen.Scale = 1;
    }
}