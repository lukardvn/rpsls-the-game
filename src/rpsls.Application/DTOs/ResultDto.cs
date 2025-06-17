using rpsls.Domain.Models;

namespace rpsls.Application.DTOs;

public record ResultDto(Choice Player, Choice Computer, Outcome Result); 