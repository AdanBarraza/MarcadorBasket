// Namespace del proyecto. Debe coincidir con x:Class="MarcadorBasket.MainPage" en el XAML.
namespace MarcadorBasket;

// MainPage es la pantalla principal. Hereda de ContentPage (página visual de MAUI).
// "partial" indica que parte de la clase está generada por el XAML.
public partial class MainPage : ContentPage
{
    // ==========================
    //  ESTADO DEL MARCADOR
    // ==========================

    int scoreLocal = 0;       // Puntos del equipo local.
    int scoreVisitante = 0;   // Puntos del equipo visitante.

    int periodo = 1;          // Periodo actual del juego (1 a 4).
    const int maxPeriodo = 4; // Número máximo de periodos.

    int tiemposLocal = 3;     // Tiempos fuera restantes del local.
    int tiemposVisitante = 3; // Tiempos fuera restantes del visitante.

    // Duración de cada cuarto (10 minutos).
    TimeSpan duracionCuarto = TimeSpan.FromMinutes(10);

    // Tiempo restante en el reloj de juego.
    TimeSpan tiempoJuegoRestante;

    // Tiempo restante en el reloj de posesión (24 segundos).
    TimeSpan tiempoPosRestante = TimeSpan.FromSeconds(24);

    // Timers que descuentan tiempo cada segundo (juego y posesión).
    IDispatcherTimer timerJuego;
    IDispatcherTimer timerPos;

    // Banderas para saber si los relojes están corriendo o pausados.
    bool juegoCorriendo = false;
    bool posesionCorriendo = false;

    // ==========================
    //  CONSTRUCTOR DE LA PÁGINA
    // ==========================
    public MainPage()
    {
        // Carga los controles definidos en MainPage.xaml y conecta x:Name con campos generados.
        InitializeComponent();

        // Inicializamos el tiempo de juego con la duración estándar del cuarto.
        tiempoJuegoRestante = duracionCuarto;

        // Pintamos en la pantalla los valores iniciales (score 0, periodo 1, tiempos 3, relojes).
        ActualizarMarcadores();

        // Creamos y configuramos el timer del juego.
        timerJuego = Dispatcher.CreateTimer();                   // Se crea el timer asociado al hilo de UI.
        timerJuego.Interval = TimeSpan.FromSeconds(1);           // Tick cada 1 segundo.
        timerJuego.Tick += TimerJuego_Tick;                      // Método a ejecutar en cada Tick.

        // Creamos y configuramos el timer de posesión.
        timerPos = Dispatcher.CreateTimer();
        timerPos.Interval = TimeSpan.FromSeconds(1);
        timerPos.Tick += TimerPos_Tick;
    }

    // ==========================
    //  ACTUALIZAR PANTALLA
    // ==========================
    // Este método toma el estado interno (variables) y lo refleja en los Label de la UI.
    void ActualizarMarcadores()
    {
        // Marcadores de puntos:
        lblScoreLocal.Text = scoreLocal.ToString();
        lblScoreVisitante.Text = scoreVisitante.ToString();

        // Periodo actual:
        lblPeriodo.Text = periodo.ToString();

        // Tiempos fuera:
        lblTiemposLocal.Text = tiemposLocal.ToString();
        lblTiemposVisitante.Text = tiemposVisitante.ToString();

        // Reloj del juego en formato mm:ss (ejemplo: 09:05).
        lblRelojJuego.Text = $"{tiempoJuegoRestante.Minutes:00}:{tiempoJuegoRestante.Seconds:00}";

        // Reloj de posesión solo en segundos (ejemplo: 24, 08, 00).
        lblRelojPosesion.Text = tiempoPosRestante.Seconds.ToString("00");
    }

    // ==========================
    //  PUNTOS LOCAL
    // ==========================
    void OnLocalMas1Clicked(object? sender, EventArgs e)
    {
        scoreLocal += 1;      // Suma 1 punto al local.
        ActualizarMarcadores();
    }

    void OnLocalMenos1Clicked(object? sender, EventArgs e)
    {
        if (scoreLocal > 0)   // Evita que baje de 0
        {
            scoreLocal -= 1;
            ActualizarMarcadores();
        }
    }

    // ==========================
    //  PUNTOS VISITANTE
    // ==========================
    void OnVisitMas1Clicked(object? sender, EventArgs e)
    {
        scoreVisitante += 1;
        ActualizarMarcadores();
    }

    void OnVisitMenos1Clicked(object? sender, EventArgs e)
    {
        if (scoreVisitante > 0)
        {
            scoreVisitante -= 1;
            ActualizarMarcadores();
        }
    }

    // ==========================
    //  TIEMPOS FUERA
    // ==========================
    void OnTiempoLocalClicked(object? sender, EventArgs e)
    {
        // Solo permite pedir tiempo si aún quedan tiempos fuera.
        if (tiemposLocal > 0)
        {
            tiemposLocal--;                      // Resta 1 tiempo fuera.
            lblMensaje.Text = "Tiempo fuera LOCAL";
            ActualizarMarcadores();
        }
        // Si llega a 0, no hace nada para evitar valores negativos.
    }

    void OnTiempoVisitClicked(object? sender, EventArgs e)
    {
        if (tiemposVisitante > 0)
        {
            tiemposVisitante--;
            lblMensaje.Text = "Tiempo fuera VISITANTE";
            ActualizarMarcadores();
        }
    }

    // ==========================
    //  CONTROL RELOJ DE JUEGO
    // ==========================
    void OnStartStopJuegoClicked(object? sender, EventArgs e)
    {
        // Alterna el estado: si estaba corriendo, se pausa; si estaba pausado, se arranca.
        juegoCorriendo = !juegoCorriendo;

        if (juegoCorriendo)
            timerJuego.Start();  // Comienza a ejecutar TimerJuego_Tick cada segundo.
        else
            timerJuego.Stop();   // Detiene el timer.
    }

    void OnResetCuartoClicked(object? sender, EventArgs e)
    {
        // Detenemos el reloj y regresamos el tiempo al inicio del cuarto.
        juegoCorriendo = false;
        timerJuego.Stop();
        tiempoJuegoRestante = duracionCuarto;
        ActualizarMarcadores();
    }

    void OnSiguientePeriodoClicked(object? sender, EventArgs e)
    {
        // Solo avanzamos si aún no llegamos al último periodo.
        if (periodo < maxPeriodo)
        {
            periodo++;                      // Pasa al siguiente periodo.
            tiempoJuegoRestante = duracionCuarto; // Reinicia el tiempo del reloj.
            juegoCorriendo = false;
            timerJuego.Stop();
            ActualizarMarcadores();
        }
    }

    // Este método se ejecuta automáticamente cada segundo cuando timerJuego está activo.
    void TimerJuego_Tick(object? sender, EventArgs e)
    {
        if (tiempoJuegoRestante.TotalSeconds > 0)
        {
            // Restamos exactamente 1 segundo al TimeSpan.
            tiempoJuegoRestante -= TimeSpan.FromSeconds(1);
            ActualizarMarcadores();
        }
        else
        {
            // Cuando el tiempo llega a 0, se detiene el juego para ese periodo.
            juegoCorriendo = false;
            timerJuego.Stop();
            lblMensaje.Text = "Fin del periodo";
        }
    }

    // ==========================
    //  CONTROL RELOJ DE POSESIÓN
    // ==========================
    void OnStartStopPosesionClicked(object? sender, EventArgs e)
    {
        // Alterna el estado del reloj de posesión.
        posesionCorriendo = !posesionCorriendo;

        if (posesionCorriendo)
            timerPos.Start();
        else
            timerPos.Stop();
    }

    void OnReset24Clicked(object? sender, EventArgs e)
    {
        // Vuelve la posesión a 24 segundos y detiene el reloj.
        tiempoPosRestante = TimeSpan.FromSeconds(24);
        posesionCorriendo = false;
        timerPos.Stop();
        ActualizarMarcadores();
    }

    // Método que se ejecuta cada segundo cuando timerPos está activo.
    void TimerPos_Tick(object? sender, EventArgs e)
    {
        if (tiempoPosRestante.TotalSeconds > 0)
        {
            // Restamos 1 segundo a la posesión.
            tiempoPosRestante -= TimeSpan.FromSeconds(1);
            ActualizarMarcadores();
        }
        else
        {
            // Si se agota el tiempo de posesión, se marca violación de 24 segundos.
            posesionCorriendo = false;
            timerPos.Stop();
            lblMensaje.Text = "Violación de 24 s";
        }
    }
}
