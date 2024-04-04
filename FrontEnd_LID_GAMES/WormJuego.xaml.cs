namespace LID_Games_Arcade;

using FrontEnd_LID_GAMES;
using FrontEnd_LID_GAMES.Entidades;
using FrontEnd_LID_GAMES.Entidades.Request;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.Text;

public partial class WormJuego : ContentPage
{
    private Image comida;
    private int puntosTotales = 0;
    private int conteoPizzas = 0;
    int tiempoDuracionGIF = 0;

    public static System.Timers.Timer tiempoWorm = new System.Timers.Timer();
    public static System.Timers.Timer tiempoManzana = new System.Timers.Timer();
    private System.Timers.Timer tiempoGIF = new System.Timers.Timer();

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private Direction currentDirection = Direction.Right;
    private List<(int Fila, int Columna)> posicionesOcupadas = new List<(int Fila, int Columna)>();
    public WormJuego()
    {
        InitializeComponent();
        crearEspacios();
        randomGoodFood();


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
    private void crearEspacios()
    {

        espacioJuegoWorm.RowDefinitions = new RowDefinitionCollection();
        espacioJuegoWorm.ColumnDefinitions = new ColumnDefinitionCollection();

        for (int i = 0; i < 25; i++)
        {
            if (i == 0 || i == 24)
            {
                espacioJuegoWorm.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Absolute)
                });
                espacioJuegoWorm.Add(new BoxView
                {
                    Color = Colors.Black
                }, i, 0);

            }
            else
            {
                espacioJuegoWorm.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(40, GridUnitType.Absolute)
                });
            }



            for (int j = 0; j < 12; j++)
            {

                if (i == 0 || i == 24)
                {

                    espacioJuegoWorm.Add(new BoxView
                    {
                        Color = Colors.Black
                    }, i, j);

                }
                else
                {
                    if (j == 0 || j == 11)
                    {
                        espacioJuegoWorm.RowDefinitions.Add(new RowDefinition
                        {
                            Height = new GridLength(1, GridUnitType.Absolute)
                        });
                        espacioJuegoWorm.Add(new BoxView
                        {
                            Color = Colors.Black
                        }, i, j);
                    }
                    else
                    {
                        espacioJuegoWorm.RowDefinitions.Add(new RowDefinition
                        {
                            Height = new GridLength(40, GridUnitType.Absolute)
                        });
                        espacioJuegoWorm.Add(new BoxView
                        {
                            Color = Colors.Transparent
                        }, i, j);

                    }

                }
            }
        }

    }

    private void tiempoDeAparicionManzana()
    {
        tiempoManzana.Interval = 3000;
        tiempoManzana.Elapsed += randomBadFood;
        tiempoManzana.Enabled = false;
        tiempoManzana.Start();

    }

    private void tiempoDeMovimientoWorm()
    {
        tiempoWorm.Interval = 300;
        tiempoWorm.Elapsed += moveWorm;
        tiempoWorm.Enabled = false;
        tiempoWorm.Start();
    }

    private void moveWorm(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            //esto para mover bicho
            int fila = Grid.GetRow(wormHead);
            int columna = Grid.GetColumn(wormHead);

            //Verificar a que direccion va a ir
            switch (currentDirection)
            {
                case Direction.Up:
                    Grid.SetRow(wormHead, fila - 1);
                    break;
                case Direction.Down:
                    Grid.SetRow(wormHead, fila + 1);
                    break;
                case Direction.Left:
                    Grid.SetColumn(wormHead, columna - 1);
                    break;
                case Direction.Right:
                    Grid.SetColumn(wormHead, columna + 1);
                    break;
            }
            collisionWorm();
            wormEat();

        });
    }

    private void collisionWorm()
    {
        int cabezaFila = Grid.GetRow(wormHead);
        int cabezaColumna = Grid.GetColumn(wormHead);

        if (cabezaFila <= 0 || cabezaFila >= 11 || cabezaColumna <= 0 || cabezaColumna >= 24)
        {
            GameOver();
            return;
        }
    }

    private void randomGoodFood()
    {

        comida = new Image
        {
            Source = "pizza_worm.png",
            Aspect = Aspect.Fill,
        };


        Random random = new Random();

        (int fila, int columna) posicion;

        do
        {
            posicion = (random.Next(1, 11), random.Next(1, 24));
        } while (PosicionOcupada(posicion));

        posicionesOcupadas.Add(posicion);


        espacioJuegoWorm.Children.Add(comida);


        Grid.SetRow(comida, posicion.fila);
        Grid.SetColumn(comida, posicion.columna);

    }

    private void randomBadFood(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            Image comidaMala = new Image
            {
                Source = "manzana_mala.png",
                Aspect = Aspect.Fill,
            };


            Random random = new Random();
            (int fila, int columna) posicion;

            do
            {
                posicion = (random.Next(1, 11), random.Next(1, 24));

            } while (PosicionOcupada(posicion));

            posicionesOcupadas.Add(posicion);


            espacioJuegoWorm.Children.Add(comidaMala);


            Grid.SetRow(comidaMala, posicion.fila);
            Grid.SetColumn(comidaMala, posicion.columna);
        });
    }

    private bool PosicionOcupada((int Fila, int Columna) posicion)
    {

        foreach (var pos in posicionesOcupadas)
        {
            if (posicion.Fila == pos.Fila && posicion.Columna == pos.Columna)
            {
                return true; // La posición está ocupada
            }
        }
        return false; // La posición no está ocupada
    }


    private void wormEat()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            int cabezaFila = Grid.GetRow(wormHead);
            int cabezaColumna = Grid.GetColumn(wormHead);
            int pizzaFila = Grid.GetRow(comida);
            int pizzaColumna = Grid.GetColumn(comida);

            foreach (var pos in posicionesOcupadas)
            {
                if (cabezaFila == pos.Fila && cabezaColumna == pos.Columna && cabezaFila != pizzaFila && cabezaColumna != pizzaColumna)
                {
                    GameOver();
                    return;
                }
            }

            if (cabezaFila == pizzaFila && cabezaColumna == pizzaColumna)
            {
                puntosTotales += 100;
                conteoPizzas += 1;
                // Suma puntos y la pizza se desplaza y llevar conteo pizzas
                if (puntosTotales <= 999)
                {
                    lblScore.Text = "0" + puntosTotales.ToString();
                }
                else
                {
                    lblScore.Text = puntosTotales.ToString();
                }

                if (conteoPizzas <= 9)
                {
                    lblPizzas.Text = "000" + conteoPizzas.ToString();
                }
                else
                {
                    lblPizzas.Text = "00" + conteoPizzas.ToString();
                }
                espacioJuegoWorm.Children.Remove(comida);
                var pizzaEliminar = posicionesOcupadas.FirstOrDefault(p => p.Columna == pizzaColumna && p.Fila == pizzaFila);

                posicionesOcupadas.Remove(pizzaEliminar);

                randomGoodFood();
            }
        });
    }

    private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
    {
        switch (e.Direction)
        {
            case SwipeDirection.Left:
                currentDirection = Direction.Left;
                break;
            case SwipeDirection.Right:
                currentDirection = Direction.Right;
                break;
            case SwipeDirection.Up:
                currentDirection = Direction.Up;
                break;
            case SwipeDirection.Down:
                currentDirection = Direction.Down;
                break;
        }
    }

    private async void GameOver()
    {
        tiempoWorm.Stop();
        tiempoManzana.Stop();
        musicaFondo.Stop();
        wormHead.Source = "worm_dead.png";
        if (puntosTotales == 0)
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

    private async Task ingresarPuntaje()
    {
        try
        {
            ReqIngresarPuntaje req = new ReqIngresarPuntaje();
            req.elPuntaje = new Puntaje();

            req.elPuntaje.idJuego = 3;
            req.elPuntaje.idUsuario = MainPage.userID;
            req.elPuntaje.puntos = puntosTotales;
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
                    await DisplayAlert("¡Felicidades!", "Usted ha obtenido un total de: " + puntosTotales.ToString() + " puntos", "Aceptar");
                    Navigation.PopAsync();
                    await Navigation.PushAsync(new LeaderBoardWorm());
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

    private void pausebtn_Clicked(object sender, EventArgs e)
    {
        audioTecla.Play();
        tiempoManzana.Stop();
        tiempoWorm.Stop();
        Navigation.PushAsync(new MenuPausaWorm());
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

    private void StartGameClicked()
    {
        tiempoGIF.Stop();
        tiempoGIF.Dispose();
        gifPlay.IsVisible = false;
        // Mostrar el contenido del juego al hacer clic en el botón de inicio
        contenidoJuego.IsVisible = true; // "ContenidoJuego" es el nombre del StackLayout que contiene el juego
        tiempoDeMovimientoWorm();
        tiempoDeAparicionManzana();
    }



}
