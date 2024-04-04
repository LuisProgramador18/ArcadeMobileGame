using CommunityToolkit.Maui;
using FrontEnd_LID_GAMES;
using FrontEnd_LID_GAMES.Entidades;
using FrontEnd_LID_GAMES.Entidades.Request;
using LID_Games_Arcade.Entidades;
using Newtonsoft.Json;
using System.Text;

namespace LID_Games_Arcade;

public partial class PacManMemoryJuego : ContentPage
{

    int tiempoTotal = 30;
    int tiempoDuracionGIF = 0;
    public static System.Timers.Timer tiempoRestante = new System.Timers.Timer();
    private System.Timers.Timer tiempoGIF = new System.Timers.Timer();
    string[] contenidos = { "carta_blue_ghost.jpg", "carta_orange_ghost.jpg", "carta_pink_ghost.jpg", "carta_red_ghost.jpg", "carta_skyblue_ghost.jpg", "carta_pacman.jpg", "carta_cherry.jpg", "carta_comida.jpg", "carta_fresa.jpg" };
    private CartaPacMan[] cartas = new CartaPacMan[18];
    int puntosObtenidos = 0;
    int contadorGiros = 0;
    CartaPacMan carta1, carta2;

    public PacManMemoryJuego()
    {
        InitializeComponent();

        asignacionRandomCartas();
        tiempoRestante.Interval = 1000;
        tiempoRestante.Elapsed += modificarTiempo;
        tiempoRestante.Enabled = false;

        tiempoGIF.Interval = 1000;
        tiempoGIF.Elapsed += contadorGIF;
        tiempoGIF.Enabled = false;
        tiempoGIF.Start();
    }


    private void contadorGIF(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>

        {
            if (tiempoDuracionGIF == 5)
            {
                StartGameClicked();
            }
            else
            {
                tiempoDuracionGIF++;
            }
        });
    }

    private void pausebtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        //Hay que buscar la manera de parar el tiempo cuando se vaya al otro menú  
        tiempoRestante.Stop();
        Navigation.PushAsync(new MenuPausaPacMan());

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


    private async void girarCarta(object sender, EventArgs e)
    {
        //Aquí trae el boton imagen sin tener que llamarlo por su nombre. Se hace para no tener que llamar a todos
        ImageButton imagen = (ImageButton)sender;


        int posicionCarta1 = 0;
        int n1 = 0;
        foreach (var card in cartas)
        {

            if (imagen.AutomationId.Equals(card.cartaQueAlmacena))
            {

                //Puede mejorarse con algo que impida que se pueda presionar 2 veces una misma carta

                if (card.estadoEncontrado != true)
                {
                    //Aquí cambia el "grosor" de la imagen. Afecta únicamente el tamaño en el eje X. El 0 hace que llegue a ser transparente, y el 200 son milisegundos. El tiempo en el que se va a ejecutar
                    await imagen.ScaleXTo(0, 200);


                    //Le cambia el origen de la imagen. Ya no es la carta de pacman, sino la cherry. Esto es provisional, pues se asigna de forma random
                    imagen.Source = card.imagenAsignada;

                    //Vuelve la imagen a la escala normal
                    await imagen.ScaleXTo(1);

                    if (contadorGiros == 1)
                    {

                        carta2 = new CartaPacMan();
                        carta2.cartaQueAlmacena = imagen.AutomationId;
                        carta2.imagenAsignada = card.imagenAsignada;
                        contadorGiros++;

                        bool resultado = verificarParidad(carta1, carta2);

                        if (resultado)
                        {
                            //Segun la prueba, el sistema sí determina si es igual o no
                            cartas[n1].estadoEncontrado = true;
                            card.estadoEncontrado = true;
                            carta1 = null;
                            carta2 = null;
                            puntosObtenidos += 100;
                            if (puntosObtenidos >= 100)
                            {
                                puntaje.Text = "0" + puntosObtenidos.ToString();
                            }
                            else
                            {
                                puntaje.Text = puntosObtenidos.ToString();
                            }

                            contadorGiros = 0;
                        }
                        else
                        {
                            cartas[n1].estadoEncontrado = false;
                            card.estadoEncontrado = false;
                            string cartaRotar1 = carta1.cartaQueAlmacena;
                            ImageButton cartaMala1 = (ImageButton)FindByName(cartaRotar1);
                            await cartaMala1.ScaleXTo(0, 200);
                            cartaMala1.Source = "pacman_card.png";


                            string cartaRotar2 = carta2.cartaQueAlmacena;
                            ImageButton cartaMala2 = (ImageButton)FindByName(cartaRotar2);
                            await cartaMala2.ScaleXTo(0, 200);
                            cartaMala2.Source = "pacman_card.png";

                            cartaMala1.ScaleXTo(1);
                            cartaMala2.ScaleXTo(1);

                            carta1 = null;
                            carta2 = null;
                            contadorGiros = 0;
                        }
                    }
                    else if (contadorGiros == 0)
                    {

                        carta1 = new CartaPacMan();
                        carta1.estadoEncontrado = false;
                        carta1.imagenAsignada = card.imagenAsignada;
                        carta1.cartaQueAlmacena = imagen.AutomationId;
                        n1 = posicionCarta1;

                        contadorGiros++;

                    }


                }
            }

            posicionCarta1++;
        }


    }

    private void modificarTiempo(object sender, EventArgs e)
    {


        MainThread.BeginInvokeOnMainThread(async () =>

        {

            if (tiempoTotal > 0)
            {
                tiempoTotal -= 1;

                if (tiempoTotal >= 10)
                {
                    temporizadorJuego.Text = "00:" + tiempoTotal.ToString();
                }
                else
                {
                    temporizadorJuego.Text = "00:0" + tiempoTotal.ToString();
                }
            }
            else
            {
                tiempoRestante.Stop();
                musicaFondo.Stop();
                if (puntosObtenidos == 0)
                {
                    await DisplayAlert("¡Suerte para la próxima!", "No has conseguido ningún punto.", "Aceptar");
                    Navigation.PopAsync();
                    Navigation.PopAsync();
                }
                else
                {
                    await ingresarPuntaje();
                }

            }

        });


    }

    private void asignacionRandomCartas()
    {

        string[] arregloCartasUsadas = new string[18];
        var random = new Random();


        for (int i = 0; i < 18; i++)
        {
            bool repetido = false;

            do
            {
                int imagenParaCarta = random.Next(contenidos.Length);
                int numCartasAsignadas = arregloCartasUsadas.Count(item => item == contenidos[imagenParaCarta]);

                if (numCartasAsignadas < 2)
                {
                    arregloCartasUsadas[i] = contenidos[imagenParaCarta];
                    CartaPacMan carta = new CartaPacMan();

                    carta.cartaQueAlmacena = "cartaPacMan" + i.ToString();
                    carta.estadoEncontrado = false;
                    carta.imagenAsignada = contenidos[imagenParaCarta];

                    cartas[i] = carta;
                    repetido = false;
                    break;
                }
                else
                {
                    repetido = true;
                }



            } while (repetido);


        }


    }


    private bool verificarParidad(CartaPacMan carta1, CartaPacMan carta2)
    {
        if (carta1.imagenAsignada == carta2.imagenAsignada)
        {

            return true;
        }
        else
        {
            return false;
        }

    }


    private async Task ingresarPuntaje()
    {
        try
        {


            ReqIngresarPuntaje req = new ReqIngresarPuntaje();
            req.elPuntaje = new Puntaje();

            req.elPuntaje.idJuego = 2;
            req.elPuntaje.idUsuario = MainPage.userID;
            req.elPuntaje.puntos = Int32.Parse(puntaje.Text);
            var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

            HttpClient httpClient = new HttpClient();

            var response = await httpClient.PostAsync("https://webapilidgames.azurewebsites.net/api/puntaje/ingresarPuntaje", jsonContent);

            if (response.IsSuccessStatusCode)//200?
            {
                //Si es 200 todo bien

                ResIngresarPuntaje res = new ResIngresarPuntaje();

                var responseContent = await response.Content.ReadAsStringAsync();

                res = JsonConvert.DeserializeObject<ResIngresarPuntaje>(responseContent);

                if (res.resultado)
                {
                    await DisplayAlert("¡Felicidades!", "Usted ha obtenido un total de: " + puntosObtenidos.ToString() + " puntos", "Aceptar");
                    tiempoTotal = 30;
                    puntosObtenidos = 0;
                    Navigation.PopAsync();
                    await Navigation.PushAsync(new LeaderBoardPacman());
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

    private void StartGameClicked()
    {
        tiempoGIF.Stop();
        tiempoGIF.Dispose();
        gifPlay.IsVisible = false;
        contenidoJuego.IsVisible = true;
        tiempoRestante.Start();
    }




}