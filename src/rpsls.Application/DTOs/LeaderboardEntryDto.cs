namespace rpsls.Application.DTOs;

public record LeaderboardEntryDto
(
    string Username,
    int Wins,
    int TotalGames,
    double WinRate
);