using System.Windows;

namespace AutoSyncTool
{
    public partial class App : Application
    {
        private const string SINGLETON_MUTEX_ID = "DBCF4E81-DF30-4FA4-9652-9FC52304D8EF";

        private Singleton? Singleton { get; set; }
        private object SingletonSync { get; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            bool singletonLocked;
            lock (SingletonSync)
            {
                Singleton = new Singleton();
                singletonLocked = Singleton.TryLock(SINGLETON_MUTEX_ID);
            }

            if (!singletonLocked)
                base.OnStartup(e);
            else
                Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            lock (SingletonSync)
            {
                Singleton?.Dispose();
                Singleton = null;
            }
            base.OnExit(e);
        }
    }
}
