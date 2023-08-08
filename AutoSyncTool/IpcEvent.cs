using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AutoSyncTool
{
    internal class IpcEvent : IDisposable
    {
        private bool Disposed { get; set; }
        private CancellationTokenSource? Cts { get; set; }
        private Task? EventTask { get; set; }

        public void SetHandler(string eventId, Func<Task> eventHandler)
        {
            Cts = new CancellationTokenSource();
            var ctx = Cts.Token;
            EventTask = Task.Run(async () => await EventProcessor(eventId, eventHandler, ctx), ctx);
        }

        public void Dispose()
        {
            if (Disposed)
                return;
            Disposed = true;
            if (EventTask == null || Cts == null)
                return;

            Cts.Cancel();
            try { EventTask.Wait(); } catch { /* ignore */ }
            EventTask.Dispose();
            Cts.Dispose();
        }

        private static async Task EventProcessor(string eventId, Func<Task> eventHandler, CancellationToken ctx)
        {
            do
            {
                await using var pipeServer = new NamedPipeServerStream(eventId);
                await pipeServer.WaitForConnectionAsync(ctx);

                try { await eventHandler(); }
                catch { /* ignore */ }

            } while (!ctx.IsCancellationRequested);
            ctx.ThrowIfCancellationRequested();
        }

        public static void Invoke(string eventId)
        {
            using var pipeClient = new NamedPipeClientStream(eventId);
            pipeClient.Connect();
        }
    }
}
