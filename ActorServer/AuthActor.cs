using Akka.Actor;

public class AuthActor : ReceiveActor
{
    public class Authenticate
    {
        public string ApiKey { get; }

        public Authenticate(string apiKey)
        {
            ApiKey = apiKey;
        }
    }

    public AuthActor()
    {
        ReceiveAsync<Authenticate>(async auth =>
        {
            var isAuthenticated = await AuthenticateAsync(auth.ApiKey);
            Sender.Tell(isAuthenticated);
        });
    }

    private Task<bool> AuthenticateAsync(string apiKey)
    {
        var validApiKeys = new[] { "valid_api_key_1", "valid_api_key_2" };
        return Task.FromResult(validApiKeys.Contains(apiKey));
    }
}


