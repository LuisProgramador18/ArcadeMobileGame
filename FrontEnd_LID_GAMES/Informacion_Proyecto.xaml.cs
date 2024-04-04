namespace LID_Games_Arcade;

public partial class Informacion_Proyecto : ContentPage
{
	public Informacion_Proyecto()
	{
		InitializeComponent();
	}

    private void ExitBtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        Navigation.PopAsync();
        
    }

    private void ExitBtn_Pressed(object sender, EventArgs e)
    {
        ExitBtn.Scale = 0.9;
    }

    private void ExitBtn_Released(object sender, EventArgs e)
    {
        ExitBtn.Scale = 1;
    }
}