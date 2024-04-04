namespace LID_Games_Arcade;

public partial class PacmanMemory : ContentPage
{
	public PacmanMemory()
	{
		InitializeComponent();
	}

    private void playBtnPac_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        Navigation.PushAsync(new PacManMemoryJuego());
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

    private void lista_puntajes_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        Navigation.PushAsync(new LeaderBoardPacman());
    }

}