using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;

namespace AutoSyncTool
{
    internal class IpcEvent : IAsyncDisposable
    {
        private bool Disposed { get; set; }
        private CancellationTokenSource Cts { get; set; }
        private Task? EventTask { get; set; }

        //private const string EVENT_PIPE_NAME = "4E7FE0FC-6CC9-4C08-ADFD-CB5173F00CC2";
        public IpcEvent(string eventId, Func<Task> eventHandler)
        {
            Cts = new CancellationTokenSource();
            var ctx = Cts.Token;
            EventTask = Task.Run(async () => await EventProcessor(eventId, eventHandler, ctx), ctx);
        }

        public async ValueTask DisposeAsync()
        {
            if (Disposed) 
                return;
            Disposed = true;
            if (EventTask == null)
                return;
            Cts.Cancel();
            try { await EventTask; }
            catch { /* ignore */ }
Cts.Dispose();        }

        private static async Task EventProcessor(string eventId, Func<Task> eventHandler, CancellationToken ctx)
        {
            var message = new Memory<byte>(new byte[1]);
            using var memoryFile = MemoryMappedFile.CreateOrOpen(eventId, message.Length);
            await using var viewStream = memoryFile.CreateViewStream();

            do
            {
                viewStream.Position = 0;
                var bytesRead = await viewStream.ReadAsync(message, ctx);
                if (bytesRead == 0)
                    continue;

                try { await eventHandler(); }
                catch { /* ignore */ }

            } while (!ctx.IsCancellationRequested);
            ctx.ThrowIfCancellationRequested();


            //long length = new FileInfo(fileName).Length;
            //using (var stream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            //{
            //    using (var mmf = MemoryMappedFile.CreateFromFile(stream, null, length, MemoryMappedFileAccess.Read, null, HandleInheritability.Inheritable, false))
            //    {
            //        using (var viewStream = mmf.CreateViewStream(0, length, MemoryMappedFileAccess.Read))
            //        {
            //            using (BinaryReader binReader = new BinaryReader(viewStream))
            //            {
            //                var result = binReader.ReadBytes((int)length);
            //                return result;
            //            }
            //        }
            //    }
            //}
        }

        private static void Invoke()
        {
            //using (var stream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            //{
            //    using (var mmf = MemoryMappedFile.CreateFromFile(stream, null, data.Length, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.Inheritable, true))
            //    {
            //        using (var view = mmf.CreateViewAccessor())
            //        {
            //            view.WriteArray(0, data, 0, data.Length);
            //        }
            //    }

            //    stream.SetLength(data.Length);  // Make sure the file is the correct length, in case the data got smaller.
            //}
        }

    }
}
