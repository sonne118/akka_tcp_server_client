using System.Net.Sockets;
using System.Text;

namespace TCP_Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string ipAddress = "localhost";
            int port = 8000;
            ExTcpClient _client;
            _client = new ExTcpClient();
            await _client.Connect(ipAddress, port);
            _client._MessageReceived += Server_MessageReceived;
            while (_client.Client.Connected)
            {
                await Task.Delay(10);
                var eventArgs = await EventHelper.WaitForEventAsync(handler => Task.Run(() =>
                                                   _client._MessageReceived += handler),
                                                   handler => _client._MessageReceived -= handler);
            }
        }
        static async Task Server_MessageReceived(object sender, string message)
        {
            Console.WriteLine($"Server response: {message}");
            await Task.CompletedTask;
        }
    }
}