using System.Net.Sockets;
using System.Text;

namespace TCP_Client
{
    public delegate Task MessageReceivedHandler(object sender, string message);
    public class ExTcpClient
    {
        public event MessageReceivedHandler _MessageReceived;
        private NetworkStream stream;
        private TcpClient _client;
        public TcpClient Client
        {
            get { return _client; }
        }

        public async Task Connect(string ipAddress, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ipAddress, port);
            stream = _client.GetStream();

            var sendApiKey = await SendApiKey();

            if (sendApiKey)
                _ = ReceiveDataAsync();
        }
        private async Task<bool> SendApiKey()
        {

            var apiKey = "valid_api_key_1";
            var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey + "\n");
            await stream.WriteAsync(apiKeyBytes, 0, apiKeyBytes.Length);

            var buffer = new byte[1024];
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Server response: {response}");

            if (response.Contains("Invalid API Key"))
            {
                return false;
            }
            return true;
        }

        private async Task ReceiveDataAsync()
        {

            byte[] buffer = new byte[1024];
            StringBuilder messageBuilder = new StringBuilder();

            while (true)
            {
                var userMessage = Console.ReadLine();
                if (string.IsNullOrEmpty(userMessage)) break;

                var userMessageBytes = Encoding.UTF8.GetBytes(userMessage + "\n");
                await stream.WriteAsync(userMessageBytes, 0, userMessageBytes.Length);


                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messageBuilder.Append(data);

                if (data.Length > 0)
                {
                    string receivedMessage = messageBuilder.ToString();

                    if (_MessageReceived != null)
                    {
                        await OnMessageReceived(receivedMessage);
                    }
                    messageBuilder.Clear();
                }
            }

            _client.Close();
        }
        public void Disconnect()
        {
            _client?.Close();
        }
        protected virtual async Task OnMessageReceived(string message)
        {
            _MessageReceived?.Invoke(this, message);
            await Task.CompletedTask;
        }
    }
}
