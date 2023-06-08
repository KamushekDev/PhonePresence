namespace PresenceBot.Infrastructure.Database;

public class DatabaseSettings
{
    public const string SectionName = "Database";
    public required string Path { get; init; }
    public required string Filename { get; init; }
}