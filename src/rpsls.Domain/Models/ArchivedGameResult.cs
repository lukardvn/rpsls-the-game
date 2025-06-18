namespace rpsls.Domain.Models;

public record ArchivedGameResult(
    Guid Id, 
    string Username, 
    Choice PlayerChoice, 
    Choice ComputerChoice, 
    Outcome Outcome, 
    DateTime PlayedAt, 
    DateTime ArchivedAt
);