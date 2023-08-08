using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AutoSyncTool
{
    public class Logger : IAsyncDisposable
    {
        private bool Disposed { get; set; }
        private static readonly string ProcessId = Environment.ProcessId.ToString("D4");
        private static readonly string LogsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory();
        private BlockingCollection<string> Messages { get; } = new();
        private CancellationTokenSource Cts { get; } = new();
        private Task MessageProcessorTask { get; set; }

        public Logger()
        {
            var ctx = Cts.Token;
            MessageProcessorTask = Task.Run(() => MessageProcessor(ctx), ctx);
        }

        public async ValueTask DisposeAsync()
        {
            if (Disposed)
                return;
            Disposed = true;
            Cts.Cancel();
            try { await MessageProcessorTask; }
            catch { /* ignore */ }
            await CastAndDispose(Cts);
            await CastAndDispose(MessageProcessorTask);
            await CastAndDispose(Messages);

            return;

            static async ValueTask CastAndDispose(IDisposable resource)
            {
                if (resource is IAsyncDisposable resourceAsyncDisposable)
                    await resourceAsyncDisposable.DisposeAsync();
                else
                    resource.Dispose();
            }
        }

        public void Write(string message)
        {
            Messages.Add($"{DateTime.Now:HH:mm:ss.fff} {ProcessId}:{Environment.CurrentManagedThreadId:D4} {message}");
        }

        private void MessageProcessor(CancellationToken ctx)
        {
            do
            {
                SaveToFile(Messages.Take(ctx));
            } while (!ctx.IsCancellationRequested);
            ctx.ThrowIfCancellationRequested();
        }

        private static void SaveToFile(string text)
        {
            try { File.AppendAllLines(Path.Combine(LogsPath, $"{DateTime.Now:yyyy-MM-dd}.log"), new []{ text }); }
            catch { /* ignore */ }
        }
    }
}
