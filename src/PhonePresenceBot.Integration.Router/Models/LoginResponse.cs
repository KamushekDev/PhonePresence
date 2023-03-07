using Newtonsoft.Json;

namespace PhonePresenceBot.Integration.Router.Models;

public record LoginResponse([property: JsonProperty("asus_token")] string AsusToken);