namespace LID_Games_Arcade;
using System.Windows;

public partial class MenuJuegos : ContentPage
{
	public MenuJuegos()
	{
		InitializeComponent();
	}

    private void juegoPacMemoryBtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        Navigation.PushAsync(new PacmanMemory());
    }

    private void alienAssaultBtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        Navigation.PushAsync(new AlienAssault());
    }

    private void juegoWormBtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        Navigation.PushAsync(new Worm());
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

    private void info_btn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        Navigation.PushAsync(new Informacion_Proyecto());
    }
}