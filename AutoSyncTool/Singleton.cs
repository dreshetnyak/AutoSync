using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AutoSyncTool
{
    internal class Singleton : IDisposable
    {
        private Mutex? SingletonMutex { get; set; }
        private object SingletonMutexSync { get; } = new();

        public bool TryLock(string mutexId)
        {
            lock (SingletonMutexSync)
            {
                SingletonMutex = new Mutex(false, "Local\\" + mutexId);
                if (SingletonMutex.WaitOne(0))
                    return true;
                SingletonMutex.Dispose();
                return false;
            }
        }

        public void Dispose()
        {
            lock (SingletonMutexSync)
            {
                if (SingletonMutex == null)
                    return;
                SingletonMutex.ReleaseMutex();
                SingletonMutex.Dispose();
                SingletonMutex = null;
            }
        }
    }
}
