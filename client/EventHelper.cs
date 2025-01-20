using TCP_Client;

namespace TCP_Client
{
    internal static class EventHelper
    {
        public static Task<(object sender, string message)> WaitForEventAsync(Func<MessageReceivedHandler, Task> addHandler, Action<MessageReceivedHandler> removeHandler)
        {
            var tcs = new TaskCompletionSource<(object, string)>();

            MessageReceivedHandler handler = null;
            handler = (sender, message) =>
            {
                removeHandler(handler);
                tcs.SetResult((sender, message));
                return tcs.Task;
            };

            addHandler(handler).ConfigureAwait(false);
            return tcs.Task;
        }
    }
}
