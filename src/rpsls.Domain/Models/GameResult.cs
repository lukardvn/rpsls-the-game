namespace rpsls.Domain.Models;

public record GameResult(
    Guid Id,
    string Username,
    Choice PlayerChoice,
    Choice ComputerChoice,
    Outcome Outcome,
    DateTime PlayedAt
);