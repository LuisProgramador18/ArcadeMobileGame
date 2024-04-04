namespace LID_Games_Arcade;

public partial class MenuPausaAlienAssault : ContentPage
{
	public MenuPausaAlienAssault()
	{
		InitializeComponent();
	}

    private void playResumebtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        juegoAlien.tiempoAJugar.Start();
        juegoAlien.tiempoMovimientoBala.Start();
        juegoAlien.tiempoMovimientoEnemigo.Start();
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