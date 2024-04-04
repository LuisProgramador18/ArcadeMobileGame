namespace LID_Games_Arcade;

public partial class AlienAssault : ContentPage
{
	public AlienAssault()
	{
		InitializeComponent();
	}

    private void playbtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
		Navigation.PushAsync(new juegoAlien());
    }

    private void botonPresionado(object sender, EventArgs e)
    {
        ImageButton imagen = (ImageButton)sender;
        imagen.Scale = 0.9;
    }

    private void botonLiberado(object sender, EventArgs e)
    {
        ImageButton imagen = (ImageButton)sender;
        imagen.Scale = 1;
    }

    private void lista_puntajes_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        Navigation.PushAsync(new LeaderBoardAlien());
    }
}