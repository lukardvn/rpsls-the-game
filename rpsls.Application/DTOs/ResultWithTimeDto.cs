using rpsls.Domain.Models;

namespace rpsls.Application.DTOs;

public record ResultWithTimeDto(Choice Player, Choice Computer, Outcome Result, DateTime PlayedAt);