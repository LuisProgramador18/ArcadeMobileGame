using FrontEnd_LID_GAMES;
using FrontEnd_LID_GAMES.Entidades;
using FrontEnd_LID_GAMES.Entidades.Request;
using LID_Games_Arcade.Entidades;
using Newtonsoft.Json;
using System.Text;

namespace LID_Games_Arcade;

public partial class juegoAlien : ContentPage
{

    public static System.Timers.Timer tiempoMovimientoEnemigo = new System.Timers.Timer();
    public static System.Timers.Timer tiempoMovimientoBala = new System.Timers.Timer();
    private System.Timers.Timer tiempoGIF = new System.Timers.Timer();
    public static System.Timers.Timer tiempoAJugar = new System.Timers.Timer();

    int tiempoRestante = 30;
    private List<Image> listaDeEnemigos = new List<Image>();

    int contadorBala = 0;
    int tiempoDuracionGIF = 0;
    int direccion = -1;
    int puntaje = 0;


    NaveAssault nave = new NaveAssault();

    public juegoAlien()
    {

        InitializeComponent();
        CrearEspaciosGrid();

        tiempoMovimientoEnemigo.Interval = 500;
        tiempoMovimientoEnemigo.Elapsed += moverEnemigos;
        tiempoMovimientoEnemigo.Enabled = false;


        tiempoMovimientoBala.Interval = nave.velocidadAtaque;
        tiempoMovimientoBala.Elapsed += moverBala;
        tiempoMovimientoBala.Enabled = false;

        tiempoAJugar.Interval = 1000;
        tiempoAJugar.Elapsed += TiempoDeJuego;
        tiempoAJugar.Enabled = false;

        tiempoGIF.Interval = 1000;
        tiempoGIF.Elapsed += contadorGIF;
        tiempoGIF.Enabled = false;
        tiempoGIF.Start();

    }

    private void TiempoDeJuego(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>

        {

            if (tiempoRestante > 0)
            {
                tiempoRestante -= 1;

                if (tiempoRestante >= 10)
                {
                    tiempoDeJuegoRestante.Text = "00:" + tiempoRestante.ToString();
                }
                else
                {
                    tiempoDeJuegoRestante.Text = "00:0" + tiempoRestante.ToString();
                }
            }
            else
            {
                tiempoAJugar.Stop();
                tiempoMovimientoBala.Stop();
                tiempoMovimientoEnemigo.Stop();
                musicaFondo.Stop();
                if (puntaje == 0)
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
        tiempoMovimientoEnemigo.Stop();
        tiempoMovimientoBala.Stop();
        tiempoAJugar.Stop();
        Navigation.PushAsync(new MenuPausaAlienAssault());

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

    private void Grid_SizeChanged(Image balaPrueba)
    {
        for (int i = 1; i < 31; i++)
        {
            string enemigo = "enemigo_" + i.ToString();
            Image enemigoImg = (Image)FindByName(enemigo);
            if (balaPrueba.X == enemigoImg.X)
            {
                balaPrueba.IsVisible = false;
                enemigoImg.IsVisible = false;
            }

        }
    }

    private void moverEnemigos(object state, EventArgs e)
    {

        MainThread.BeginInvokeOnMainThread(() =>
        {
            for (int i = 0; i < listaDeEnemigos.Count; i++)
            {
                if (listaDeEnemigos[i] != null)
                {
                    int fila = Grid.GetRow(listaDeEnemigos[i]);
                    int columna = Grid.GetColumn(listaDeEnemigos[i]);

                    int nuevaColumna = columna + direccion;

                    if (nuevaColumna >= 0 && nuevaColumna < gridAliens.ColumnDefinitions.Count)
                    {
                        Grid.SetColumn(listaDeEnemigos[i], nuevaColumna);
                    }
                }
            }

            if (direccion == 1)
            {
                direccion = -1;
            }
            else
            {
                direccion = 1;

            }
        });

    }



    private void moverBala(object state, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            bool resultado = ValidarPosicionBalaEnemigo();
            if (contadorBala == 6 || resultado)
            {
                contadorBala = 0;

                balaPrueba.IsVisible = false;

                int posicionNave = (int)sldNave.Value;
                Grid.SetRow(balaPrueba, 6);
                Grid.SetColumn(balaPrueba, posicionNave);
                balaPrueba.IsVisible = true;
            }
            else
            {
                contadorBala++;

                balaPrueba.IsVisible = true;
                int filaActual = Grid.GetRow(balaPrueba);
                Grid.SetRow(balaPrueba, filaActual - 1);
            }

        });

    }

    private bool ValidarPosicionBalaEnemigo()
    {
        bool mismaPosicion = false;
        int balaFila = Grid.GetRow(balaPrueba);
        int balaColumna = Grid.GetColumn(balaPrueba);

        for (int i = 0; i < 45; i++)
        {
            Image enemy = listaDeEnemigos[i];
            int enemigoFila = Grid.GetRow(enemy);
            int enemigoColunma = Grid.GetColumn(enemy);
            if (balaPrueba.IsVisible && enemy.IsVisible && CheckCollision(balaFila, balaColumna, enemigoFila, enemigoColunma))
            {
                // Colisión detectada, desactivar bala y enemigo
                mismaPosicion = true;
                enemy.IsVisible = false;
                puntaje += 100;
                if (puntaje <= 999)
                {
                    lblPuntos.Text = "0" + puntaje.ToString();
                }
                else
                {
                    lblPuntos.Text = puntaje.ToString();
                }
            }

        }

        return mismaPosicion;

    }
    private bool CheckCollision(int balaFila, int balaColumna, int enemigoFila, int enemigoColumna)
    {
        // Verificar si la posición de la bala coincide con la posición del enemigo
        return balaFila == enemigoFila && balaColumna == enemigoColumna;
    }


    private void CrearEspaciosGrid()
    {
        int contador = 0;
        //i = columnas = 18 col
        for (int i = 0; i < 18; i++)
        {
            gridAliens.AddColumnDefinition(new ColumnDefinition
            {
                Width = new GridLength(45, GridUnitType.Absolute)
            });
            //j = filas = 8 filas. 
            for (int j = 0; j < 8; j++)
            {
                gridAliens.AddRowDefinition(new RowDefinition
                {
                    Height = new GridLength(45, GridUnitType.Absolute)
                });
                if (i == 1 || i == 3 || i == 5 || i == 7 || i == 9 || i == 11 || i == 13 || i == 15 || i == 17)
                {
                    if (j != 5 && j != 6 && j != 7)
                    {
                        Image nuevoEnemigo = new Image
                        {
                            Source = ImageSource.FromFile(asignacionTipoAlienRandom()),
                            WidthRequest = 45,
                            HeightRequest = 45,
                            AutomationId = "enemigo_" + contador.ToString()
                        };
                        gridAliens.Add(
                            nuevoEnemigo, i, j);
                        listaDeEnemigos.Add(nuevoEnemigo);
                        contador++;
                    }

                }

            }
        }
    }

    private string asignacionTipoAlienRandom()
    {
        var random = new Random();
        int tipoEnemigo = random.Next(1, 4);

        switch (tipoEnemigo)
        {
            case 1:
                return "enemigo_uno.png";
                break;
            case 2:
                return "enemigo_dos.png";
                break;
            case 3:
                return "enemigo_tres.png";
                break;
            default:
                return "enemigo_uno.png";
                break;
        }
    }
    private void StartGameClicked()
    {
        tiempoGIF.Stop();

        tiempoGIF.Dispose();
        gifPlay.IsVisible = false;
        // Mostrar el contenido del juego al hacer clic en el botón de inicio
        contenidoJuego.IsVisible = true; // "ContenidoJuego" es el nombre del StackLayout que contiene el juego
        tiempoMovimientoEnemigo.Start();
        tiempoMovimientoBala.Start();
        tiempoAJugar.Start();
    }

    private async Task ingresarPuntaje()
    {
        try
        {
            ReqIngresarPuntaje req = new ReqIngresarPuntaje();
            req.elPuntaje = new Puntaje();

            req.elPuntaje.idJuego = 1;
            req.elPuntaje.idUsuario = MainPage.userID;
            req.elPuntaje.puntos = puntaje;
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
                    await DisplayAlert("¡Felicidades!", "Usted ha obtenido un total de: " + puntaje.ToString() + " puntos", "Aceptar");
                    puntaje = 0;
                    tiempoRestante = 30;
                    Navigation.PopAsync();
                    await Navigation.PushAsync(new LeaderBoardAlien());
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