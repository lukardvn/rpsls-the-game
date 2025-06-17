using rpsls.Application.Common.Messaging;
using rpsls.Application.DTOs;

namespace rpsls.Application.Commands;

public record UserPlayCommand(int Choice, string? Username) : ICommand<ResultDto>;