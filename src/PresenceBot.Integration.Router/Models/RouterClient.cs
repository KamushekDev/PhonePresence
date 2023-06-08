namespace PresenceBot.Integration.Router.Models;

public record RouterClient(
    string Name,
    string NickName,
    string Ip,
    string Mac,
    string IpMethod
);