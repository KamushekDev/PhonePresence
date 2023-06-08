using PresenceBot.Integration.Router.Options;

namespace PresenceBot.Integration.Router;

public static class RouterHelper
{
    public static void ConfigureHttpClient(HttpClient client, RouterOptions options)
    {
        client.DefaultRequestHeaders.Add("user-agent", "asusrouter-Android-DUTUtil-1.0.0.245");
        client.BaseAddress = options.RouterUri;
    }
}