namespace PresenceBot.Infrastructure.VK.Options;

public class VkOptions
{
    public const string SectionName = "Vkontakte";

    public required string ApiKey { get; set; } 
    public required ulong GroupId { get; set; }
}