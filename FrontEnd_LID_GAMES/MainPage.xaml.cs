using CommunityToolkit.Maui.Core.Primitives;
using FrontEnd_LID_GAMES.Entidades;
using FrontEnd_LID_GAMES.Entidades.Response;
using LID_Games_Arcade;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FrontEnd_LID_GAMES
{
    public partial class MainPage : ContentPage
    {
        public static int userID;

        public MainPage()
        {
            InitializeComponent();
            
        }

        private void botonStart_Pressed(object sender, EventArgs e)
        {
            botonStart.Scale = 0.9;
        }

        private void botonStart_Released(object sender, EventArgs e)
        {
            botonStart.Scale = 1;
        }

        private void ExitBtn_Pressed(object sender, EventArgs e)
        {
            ExitBtn.Scale = 0.9;
        }

        private void ExitBtn_Released(object sender, EventArgs e)
        {
            ExitBtn.Scale = 1;
        }

        private void botonStart_Clicked(object sender, EventArgs e)
        {
            audioTecla.Play();
            this.enviarNickname();
           
        }

        private void nameUser_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            if (audioTecla.CurrentState == MediaElementState.Playing)
            {
                audioTecla.Stop();
                audioTecla.Play();
            }
            audioTecla.Play();

        }

        private void ExitBtn_Clicked(object sender, EventArgs e)
        {
            audioTecla.Play();
            System.Threading.Thread.Sleep(1300);
            Environment.Exit(0);
        }


        private async Task enviarNickname()
        {
            try
            {


                ReqEncontrarUsuario req = new ReqEncontrarUsuario();
                req.elUsuario = new Usuario();

                req.elUsuario.nombreUsuario = nameUser.Text;
                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                HttpClient httpClient = new HttpClient();

                var response = await httpClient.PostAsync("https://webapilidgames.azurewebsites.net/api/usuario/encontrarUsuario", jsonContent);

                if (response.IsSuccessStatusCode)//200?
                {
                    //Si es 200 todo bien

                    ResEncontrarUsuario res = new ResEncontrarUsuario();

                    var responseContent = await response.Content.ReadAsStringAsync();

                    res = JsonConvert.DeserializeObject<ResEncontrarUsuario>(responseContent);
                   
                    if (res.resultado)
                    {
                       
                        
                        userID = res.idReturn;

                        //Pasar a la página de mensajes
                        await Navigation.PushAsync(new MenuJuegos());
                    }
                    else
                    {
                        if (res.errorMensaje == "Usuario no encontrado")
                        {
                            bool respuesta = await DisplayAlert("Usuario no encontrado", "¿Desea registrar este nuevo usuario?", "Sí", "No");
                            if (respuesta)
                            {
                                ingresarUsuario();
                            }
                        }
                        else
                        {
                            await DisplayAlert("Error en backend", "Backend respondió: " + res.errorMensaje, "Aceptar");
                        }


                    }
                }
                else
                {
                    await DisplayAlert("Error de conexion", "No se pudo establecer conexion", "Aceptar");

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error sin identificar", "El error fue: " + ex.Message, "Aceptar");
            }
        }


        private async Task ingresarUsuario()
        {
            try
            {


                ReqIngresarUsuario req = new ReqIngresarUsuario();
                req.elUsuario = new Usuario();

                req.elUsuario.nombreUsuario = nameUser.Text;
                var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                HttpClient httpClient = new HttpClient();

                var response = await httpClient.PostAsync("https://webapilidgames.azurewebsites.net/api/usuario/ingresarUsuarios", jsonContent);

                if (response.IsSuccessStatusCode)//200?
                {
                    //Si es 200 todo bien

                    ResIngresarPuntaje res = new ResIngresarPuntaje();

                    var responseContent = await response.Content.ReadAsStringAsync();

                    res = JsonConvert.DeserializeObject<ResIngresarPuntaje>(responseContent);

                    if (res.resultado)
                    {

                        userID = res.idReturn;
                        await Navigation.PushAsync(new MenuJuegos());
                    }
                    else
                    {
                       
                            await DisplayAlert("Error en backend", "Backend respondió: " + res.errorMensaje, "Aceptar");


                    }
                }
                else
                {
                    await DisplayAlert("Error de conexion", "No se pudo establecer conexion", "Aceptar");

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error interno no controlado", "Error en la aplicacion" + ex.StackTrace.ToString(), "Aceptar");
            }
        }


    }
}