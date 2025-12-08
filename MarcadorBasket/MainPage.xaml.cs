// Namespace donde vive esta clase. Debe coincidir con lo puesto en x:Class del XAML.
namespace MarcadorBasket;

// Esta clase representa la pantalla principal de la app y hereda de ContentPage.
// "partial" significa que parte de la clase está definida en el archivo generado del XAML.
public partial class MainPage : ContentPage
{
    // ================================================
    //        VARIABLES DEL ESTADO DEL MARCADOR
    // ================================================

    int scoreLocal = 0;         // Guarda los puntos del equipo local.
    int scoreVisitante = 0;     // Guarda los puntos del equipo visitante.

    int periodo = 1;            // Número del periodo actual, iniciando en 1.
    const int maxPeriodo = 4;   // Cantidad máxima de periodos permitidos.

    int tiemposLocal = 3;       // Cantidad de tiempos fuera restantes del local.
    int tiemposVisitante = 3;   // Cantidad de tiempos fuera restantes del visitante.

    // Duración total del cuarto en minutos (TimeSpan permite manejar horas, minutos y segundos).
    TimeSpan duracionCuarto = TimeSpan.FromMinutes(10);

    // Tiempo restante del reloj de juego. Se inicializa luego en el constructor.
    TimeSpan tiempoJuegoRestante;

    // Tiempo restante del reloj de posesión (24 segundos al iniciar).
    TimeSpan tiempoPosRestante = TimeSpan.FromSeconds(24);

    // Timers que ejecutan una acción cada cierto intervalo (usamos 1 segundo).
    IDispatcherTimer timerJuego;
    IDispatcherTimer timerPos;

    // Banderas que indican si el reloj está corriendo o detenido.
    bool juegoCorriendo = false;
    bool posesionCorriendo = false;

    // ================================================
    //         CONSTRUCTOR DE LA PÁGINA
    // ================================================
    public MainPage()
    {
        // Carga todos los controles definidos en el archivo MainPage.xaml.
        InitializeComponent();

        // Copiamos la duración del cuarto a la variable del tiempo restante del juego.
        // Esto hace que el reloj empiece en 10:00.
        tiempoJuegoRestante = duracionCuarto;

        // Mandamos a actualizar todos los labels para que muestren valores iniciales.
        ActualizarMarcadores();

        // ============================
        // CONFIGURAR TIMER DE JUEGO
        // ============================
        timerJuego = Dispatcher.CreateTimer();               // Crea un timer asociado al hilo principal.
        timerJuego.Interval = TimeSpan.FromSeconds(1);       // Timer ejecuta Tick cada 1 segundo.
        timerJuego.Tick += TimerJuego_Tick;                  // Vinculamos el método a ejecutar cada segundo.

        // ============================
        // CONFIGURAR TIMER DE POSESIÓN
        // ============================
        timerPos = Dispatcher.CreateTimer();
        timerPos.Interval = TimeSpan.FromSeconds(1);         // Mismo intervalo de 1 segundo.
        timerPos.Tick += TimerPos_Tick;                      // Método de posesión se ejecuta cada segundo.
    }

    // ================================================
    //   MÉTODO PARA REFRESCAR TODOS LOS LABELS EN PANTALLA
    // ================================================
    void ActualizarMarcadores()
    {
        // Convierte el puntaje a texto y lo coloca en el label.
        lblScoreLocal.Text = scoreLocal.ToString();
        lblScoreVisitante.Text = scoreVisitante.ToString();

        // Muestra el periodo actual.
        lblPeriodo.Text = periodo.ToString();

        // Muestra cuántos tiempos fuera le quedan a cada equipo.
        lblTiemposLocal.Text = tiemposLocal.ToString();
        lblTiemposVisitante.Text = tiemposVisitante.ToString();

        // Formateamos el reloj de juego a mm:ss con dos dígitos. Ejemplo: 09:05
        lblRelojJuego.Text = $"{tiempoJuegoRestante.Minutes:00}:{tiempoJuegoRestante.Seconds:00}";

        // Para la posesión solo mostramos los segundos con dos dígitos.
        lblRelojPosesion.Text = tiempoPosRestante.Seconds.ToString("00");
    }

    // ================================================
    //         BOTONES PARA SUMAR AL LOCAL
    // ================================================
    void OnLocalMas1Clicked(object? sender, EventArgs e)
    {
        scoreLocal += 1;       // Suma 1 punto al marcador local.
        ActualizarMarcadores(); // Refresca la UI.
    }

    void OnLocalMas2Clicked(object? sender, EventArgs e)
    {
        scoreLocal += 2;       // Suma 2 puntos al marcador local.
        ActualizarMarcadores();
    }

    void OnLocalMas3Clicked(object? sender, EventArgs e)
    {
        scoreLocal += 3;       // Suma 3 puntos al marcador local.
        ActualizarMarcadores();
    }

    // ================================================
    //      BOTONES PARA SUMAR AL VISITANTE
    // ================================================
    void OnVisitMas1Clicked(object? sender, EventArgs e)
    {
        scoreVisitante += 1;
        ActualizarMarcadores();
    }

    void OnVisitMas2Clicked(object? sender, EventArgs e)
    {
        scoreVisitante += 2;
        ActualizarMarcadores();
    }

    void OnVisitMas3Clicked(object? sender, EventArgs e)
    {
        scoreVisitante += 3;
        ActualizarMarcadores();
    }

    // ================================================
    //           TIEMPOS FUERA
    // ================================================
    void OnTiempoLocalClicked(object? sender, EventArgs e)
    {
        // IF: solo ejecuta el bloque si la condición es verdadera.
        // tiempoLocal > 0 significa que aún quedan tiempos fuera disponibles.
        if (tiemposLocal > 0)
        {
            tiemposLocal--;                        // Resta un tiempo fuera (tiemposLocal = tiemposLocal - 1).
            lblMensaje.Text = "Tiempo fuera LOCAL"; // Muestra mensaje en pantalla.
            ActualizarMarcadores();                // Actualiza el contador visual.
        }
        // Si tiemposLocal == 0, NO entra al if y no hace nada.
    }

    void OnTiempoVisitClicked(object? sender, EventArgs e)
    {
        if (tiemposVisitante > 0)                 // Evalúa si aún tiene tiempos fuera.
        {
            tiemposVisitante--;                   // Si sí tiene, le resta uno.
            lblMensaje.Text = "Tiempo fuera VISITANTE";
            ActualizarMarcadores();
        }
    }

    // ================================================
    //         CONTROL DEL RELOJ DE JUEGO
    // ================================================
    void OnStartStopJuegoClicked(object? sender, EventArgs e)
    {
        // Alternamos el estado del booleano.
        // Si estaba en true, pasa a false. Si estaba en false, pasa a true.
        juegoCorriendo = !juegoCorriendo;

        // IF para decidir si arrancar o pausar el timer.
        if (juegoCorriendo)
            timerJuego.Start();      // Inicia el timer (TimerJuego_Tick empieza cada 1s)
        else
            timerJuego.Stop();       // Detiene el timer (ya no llama a TimerJuego_Tick).
    }

    void OnResetCuartoClicked(object? sender, EventArgs e)
    {
        juegoCorriendo = false;        // Marcamos que ya no está corriendo.
        timerJuego.Stop();             // Detenemos el timer.

        // Volvemos a dejar el tiempo restante igual que la duración del cuarto.
        tiempoJuegoRestante = duracionCuarto;

        ActualizarMarcadores();        // Refrescamos el UI.
    }

    void OnSiguientePeriodoClicked(object? sender, EventArgs e)
    {
        // IF: solo avanza el periodo si aún no alcanzamos el máximo.
        if (periodo < maxPeriodo)
        {
            periodo++;                          // Incrementa en 1 (periodo = periodo + 1).
            tiempoJuegoRestante = duracionCuarto; // Reinicia el tiempo del cuarto.
            juegoCorriendo = false;             // Se detiene el reloj.
            timerJuego.Stop();
            ActualizarMarcadores();
        }
        // Si periodo == maxPeriodo NO hace nada.
    }

    // ================================================
    //   MÉTODO QUE SE EJECUTA CADA 1 SEGUNDO (juego)
    // ================================================
    void TimerJuego_Tick(object? sender, EventArgs e)
    {
        // IF: revisa si todavía hay tiempo restante.
        if (tiempoJuegoRestante.TotalSeconds > 0)
        {
            // Resta exactamente 1 segundo al TimeSpan.
            // TimeSpan no se modifica directamente; se crea un nuevo TimeSpan restando 1 segundo.
            tiempoJuegoRestante -= TimeSpan.FromSeconds(1);

            // Actualiza los labels para mostrar el nuevo valor.
            ActualizarMarcadores();
        }
        else
        {
            // Cuando el tiempo llega a 0 segundos:
            juegoCorriendo = false;      // Marcamos que se debe detener el reloj.
            timerJuego.Stop();           // Detenemos el timer.
            lblMensaje.Text = "Fin del periodo";  // Mostramos mensaje.
        }
    }

    // ================================================
    //        CONTROL DEL RELOJ DE POSESIÓN
    // ================================================
    void OnStartStopPosesionClicked(object? sender, EventArgs e)
    {
        // Alterna entre iniciar y pausar el reloj.
        posesionCorriendo = !posesionCorriendo;

        if (posesionCorriendo)
            timerPos.Start();    // Inicia el timer
        else
            timerPos.Stop();     // Lo detiene
    }

    void OnReset24Clicked(object? sender, EventArgs e)
    {
        // Vuelve a poner la posesión en 24 segundos.
        tiempoPosRestante = TimeSpan.FromSeconds(24);

        posesionCorriendo = false; // Aseguramos que está pausado.
        timerPos.Stop();

        ActualizarMarcadores();  // Refresca UI.
    }

    // ================================================
    // MÉTODO QUE SE EJECUTA CADA 1s EN LA POSESIÓN
    // ================================================
    void TimerPos_Tick(object? sender, EventArgs e)
    {
        // IF: si aún quedan segundos en la posesión.
        if (tiempoPosRestante.TotalSeconds > 0)
        {
            // Restamos 1 segundo exactamente igual que el reloj de juego.
            tiempoPosRestante -= TimeSpan.FromSeconds(1);
            ActualizarMarcadores();     // Refrescamos pantalla.
        }
        else
        {
            // Si el reloj llegó a 0, significa violación de 24 segundos.
            posesionCorriendo = false; // Detenemos la bandera.
            timerPos.Stop();           // Detenemos el timer.
            lblMensaje.Text = "Violación de 24 s"; // Mostramos mensaje.
        }
    }
}

