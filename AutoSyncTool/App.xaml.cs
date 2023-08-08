using System.Threading.Tasks;
using System.Windows;

namespace AutoSyncTool
{
    public partial class App : Application
    {
        private const string SINGLETON_MUTEX_ID = "DBCF4E81-DF30-4FA4-9652-9FC52304D8EF";
        private const string OTHER_INSTANCE_OPEN_EVENT_ID = "4E7FE0FC-6CC9-4C08-ADFD-CB5173F00CC2";

        public static Logger Log { get; } = new();

        private Singleton Singleton { get; } = new(SINGLETON_MUTEX_ID);
        private IpcEvent OtherInstanceOpenEvent { get; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            if (Singleton.TryLock())
            {
                base.OnStartup(e);
                OtherInstanceOpenEvent.SetHandler(OTHER_INSTANCE_OPEN_EVENT_ID, OnOtherInstanceOpenEvent);
            }
            else
            {
                IpcEvent.Invoke(OTHER_INSTANCE_OPEN_EVENT_ID);
                Current.Shutdown();
            }
        }

        private static async Task OnOtherInstanceOpenEvent()
        {
            await Current.Dispatcher.InvokeAsync(() =>
            {
                var mainWindow = Current.MainWindow;
                if (mainWindow == null)
                    return;
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Activate();
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            OtherInstanceOpenEvent.Dispose();
            Singleton.Dispose();
            base.OnExit(e);
        }
    }
}
