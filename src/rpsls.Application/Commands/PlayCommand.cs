using rpsls.Application.Common.Messaging;
using rpsls.Application.DTOs;

namespace rpsls.Application.Commands;

public record PlayCommand(int Choice, string? Username) : ICommand<ResultDto>;