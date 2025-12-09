using Android.App;          
using Android.Content.PM;   
using Android.OS;          
namespace MarcadorBasket;


[Activity(

    // Tema inicial mientras carga la app.
    Theme = "@style/Maui.SplashTheme",

    // Indica que esta es la activity principal que se abrirá al iniciar la app.
    MainLauncher = true,

    // -------------------------------------------------------------
    // SIN ESTA LÍNEA LA APP GIRA AUTOMÁTICAMENTE.
    // AQUÍ BLOQUEAMOS LA APLICACIÓN SOLO EN MODO HORIZONTAL.
    // -------------------------------------------------------------
    ScreenOrientation = ScreenOrientation.Landscape
)]
public class MainActivity : MauiAppCompatActivity
{
}

