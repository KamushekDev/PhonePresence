using Newtonsoft.Json;

namespace PresenceBot.Integration.Router.Models;

public record LoginResponse([property: JsonProperty("asus_token")] string AsusToken);