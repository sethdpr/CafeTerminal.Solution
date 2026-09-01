using Android.App;
using Android.Runtime;

namespace CafeTerminal.Maui
{
    [Application]
    // Android application class that boots the shared MAUI app builder.
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        // Creates the shared MAUI app configuration used by Android.
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
