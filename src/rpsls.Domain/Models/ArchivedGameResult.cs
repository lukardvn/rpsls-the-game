namespace rpsls.Domain.Models;

public record ArchivedGameResult
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public Choice PlayerChoice { get; set; }
    public Choice ComputerChoice { get; set; }
    public Outcome Outcome { get; set; }
    public DateTime PlayedAt { get; set; }
    public DateTime ArchivedAt { get; set; }
}