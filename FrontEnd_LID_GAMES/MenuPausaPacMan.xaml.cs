namespace LID_Games_Arcade;

public partial class MenuPausaPacMan : ContentPage
{
	public MenuPausaPacMan()
	{
		InitializeComponent();
	}

    private void playResumePacbtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        PacManMemoryJuego.tiempoRestante.Start();
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