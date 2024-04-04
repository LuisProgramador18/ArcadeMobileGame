using FrontEnd_LID_GAMES.Entidades;
using LID_Games_Arcade.Entidades;
using Newtonsoft.Json;

namespace LID_Games_Arcade;

public partial class LeaderBoardPacman : ContentPage
{
	public LeaderBoardPacman()
	{
		InitializeComponent();
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarPuntajes();
    }

    private async Task CargarPuntajes()
    {
        List<Puntaje> puntajePacman = new List<Puntaje>();
        try
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://webapilidgames.azurewebsites.net/api/puntaje/obtenerPuntaje");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                ResObtenerPuntaje res = new ResObtenerPuntaje();
                res = JsonConvert.DeserializeObject<ResObtenerPuntaje>(responseContent);

                if (res.resultado)
                {
                    int position = 0;
                    foreach (var item in res.ListaDePuntajes)
                    {

                        if (item.idJuego == 2)
                        {
                            position++;
                            switch (position)
                            {
                                case 1:
                                    item.posicion = "trofeo_dorado.png";
                                    break;
                                case 2:
                                    item.posicion = "trofeo_plata.png";
                                    break;
                                case 3:
                                    item.posicion = "trofeo_bronce.png";
                                    break;
                                default:
                                    item.posicion = "medalla_leaderboard.png";
                                    break;
                            }
                            if (position == 11)
                            {
                                break;
                            }
                            puntajePacman.Add(item);
                        }

                    }

                    //Aquiiiii
                    listScores.ItemsSource = puntajePacman;


                }
                else
                {
                    await DisplayAlert("Error en backend", "Backend respondió: " + res.errorMensaje, "Aceptar");
                }
            }
            else
            {
                await DisplayAlert("No se econtro el BackEnd", "Error en la aplicacion con el EndPoint", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error interno", "Error en la aplicacion: " + ex.Message.ToString(), "Aceptar");
        }
    }

    private void quitLeaderboardBtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        Navigation.PopAsync();
        Navigation.PushAsync(new MenuJuegos());
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


