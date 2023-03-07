using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhonePresenceBot.Integration.Router.Exceptions;
using PhonePresenceBot.Integration.Router.Models;
using PhonePresenceBot.Integration.Router.Options;

namespace PhonePresenceBot.Integration.Router;

// todo: limit rps
public class RouterGateway
{
    private readonly RouterAuthOptions _authOptions;
    private readonly HttpClient _client;

    public RouterGateway(RouterAuthOptions authOptions, HttpClient client)
    {
        _authOptions = authOptions;
        _client = client;
    }

    private string LoginDataToBase64(string login, string password)
    {
        var bytes = Encoding.UTF8.GetBytes($"{login}:{password}");
        return Convert.ToBase64String(bytes);
    }

    public async Task<string> Login(CancellationToken cancellationToken)
    {
        const string loginMethodName = "login.cgi";

        var loginDataBase64 = LoginDataToBase64(_authOptions.Login, _authOptions.Password);
        var payload = new StringContent($"login_authorization={loginDataBase64}");

        var response = await _client.PostAsync(loginMethodName, payload, cancellationToken);

        var contentString = await response.Content.ReadAsStringAsync(cancellationToken);

        var result = JsonConvert.DeserializeObject<LoginResponse>(contentString);

        return result?.AsusToken ?? throw new RouterLoginFailed(contentString.ReplaceLineEndings(" ").Trim());
    }

    public async Task<List<Client>> GetClientList(CancellationToken cancellationToken)
    {
        const string hookName = "hook=get_clientlist();";
        const string methodName = "appGet.cgi";
        const string maclistName = "maclist";
        const string resultPropertyName = "get_clientlist";

        var payload = new StringContent(hookName);

        var response = await _client.PostAsync(methodName, payload, cancellationToken);

        var content2 = await response.Content.ReadAsStringAsync(cancellationToken);

        var json2 = JObject.Parse(content2);

        var clients = json2[resultPropertyName];

        var parsedClients = new List<Client>();

        foreach (JProperty c in clients)
        {
            if (c.Name == maclistName)
                continue;

            var parsedClient = JsonConvert.DeserializeObject<GetClientListResponse.Client>(c.Value.ToString());

            if (parsedClient is not null)
            {
                var client = MapClient(parsedClient);
                parsedClients.Add(client);
            }
        }

        return parsedClients;
    }

    private static Client MapClient(GetClientListResponse.Client client)
    {
        return new Client(
            client.Name,
            client.NickName,
            client.Ip,
            client.Mac,
            client.IpMethod
        );
    }
}