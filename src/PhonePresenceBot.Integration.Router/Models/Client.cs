namespace PhonePresenceBot.Integration.Router.Models;

public record Client(
    string Name,
    string NickName,
    string Ip,
    string Mac,
    string IpMethod
);