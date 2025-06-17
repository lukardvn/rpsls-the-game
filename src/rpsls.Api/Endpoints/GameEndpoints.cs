using MediatR;
using Microsoft.AspNetCore.Mvc;
using rpsls.Api.DTOs;
using rpsls.Api.Helpers;
using rpsls.Application.Commands;
using rpsls.Application.DTOs;
using rpsls.Application.Queries;

namespace rpsls.Api.Endpoints;

public static class GameEndpoints
{
    public static void RegisterGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/game").WithTags("Game");
        
        group.MapGet("/choices", async (ISender mediator, CancellationToken ct = default) =>
        {
            var result = await mediator.Send(new ChoicesQuery(), ct);
            return Results.Ok(result);
        })
        .WithDescription("Get all the choices that are usable for the UI.")
        .WithName("GetGameChoices")
        .Produces<IEnumerable<ChoiceDto>>();
        
        group.MapGet("/choice", async (ISender mediator, CancellationToken ct = default) =>
        {
            var result = await mediator.Send(new RandomChoiceQuery(), ct);
            return Results.Ok(result);
        })
        .WithDescription("Get a randomly generated choice.")
        .WithName("GetRandomChoice")
        .Produces<ChoiceDto>();
        
        //TODO: remove once /user-play is completed
        group.MapPost("/play", async (PlayRequest request, ISender mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(request.ToPlayCommand(), ct);
            return Results.Ok(result);
        })
        .WithDescription("Play a round against a computer opponent.")
        .WithName("PlayGame")
        .Accepts<PlayRequest>("application/json")
        .Produces<ResultDto>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
        
        group.MapPost("/user-play", async (UserPlayRequest request, ISender mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(request.ToPlayCommand(), ct);
            return Results.Ok(result);
        })
        .WithDescription("Play a round against a computer opponent.")
        .WithName("PlayUserGame")
        .Accepts<UserPlayRequest>("application/json")
        .Produces<ResultDto>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
        
        group.MapGet("/scoreboard", async (ISender mediator, int count = 10, CancellationToken ct = default) =>
        {
            var result = await mediator.Send(new ScoreboardQuery(count), ct);
            return Results.Ok(result);
        })
        .WithDescription("Get scoreboard of most recent results.")
        .WithName("GetScoreboard")
        .Produces<IEnumerable<ResultWithTimeDto>>()
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
        
        group.MapPost("/scoreboard/reset", async (IMediator mediator) =>
        {
            await mediator.Send(new ResetScoreboardCommand());
            return Results.Ok("Scoreboard reset successfully.");
        })
        .WithDescription("Reset scoreboard.")
        .WithName("ResetScoreboard")
        .Produces(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}