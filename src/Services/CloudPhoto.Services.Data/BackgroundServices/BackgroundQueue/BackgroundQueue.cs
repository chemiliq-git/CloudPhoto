namespace CloudPhoto.Services.Data.BackgroundServices.BackgroundQueue
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public class BackgroundQueue<T> : IBackgroundQueue<T>, IDisposable
        where T : class
    {
        private readonly ConcurrentQueue<T> items = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim signal = new SemaphoreSlim(0);

        public void Enqueue(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            items.Enqueue(item);
            signal.Release();
        }

        public async Task<T> Dequeue()
        {
            await signal.WaitAsync();
            var success = items.TryDequeue(out var workItem);

            return success
                ? workItem
                : null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                signal.Dispose();
            }
        }
    }
}
