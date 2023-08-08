using System;
using System.Threading;

namespace AutoSyncTool
{
    internal class Singleton : IDisposable
    {
        private bool Disposed { get; set; }
        private bool MutexTaken { get; set; }
        private Mutex SingletonMutex { get; set; }
        private object SingletonMutexSync { get; } = new();

        public Singleton(string mutexId)
        {
            SingletonMutex = new Mutex(false, "Local\\" + mutexId);
        }

        public bool TryLock()
        {
            lock (SingletonMutexSync)
                return MutexTaken = SingletonMutex.WaitOne(0);
        }

        public void Dispose()
        {
            lock (SingletonMutexSync)
            {
                if (Disposed)
                    return;
                Disposed = true;
                if (MutexTaken) 
                    SingletonMutex.ReleaseMutex();
                SingletonMutex.Dispose();
            }
        }
    }
}
