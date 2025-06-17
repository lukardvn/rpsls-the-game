using rpsls.Api.DTOs;
using rpsls.Application.Commands;

namespace rpsls.Api.Helpers;

/// <summary>
/// Probably no need for these.
/// Just wanted to showcase if objects were more complex and some actual mapping from Request/Response to Command was needed.
/// </summary>
public static class Mappers
{
    public static PlayCommand ToPlayCommand(this PlayRequest request) =>
        new(request.Player);
    
    public static UserPlayCommand ToPlayCommand(this UserPlayRequest request) =>
        new(request.Choice, request.Username);
}