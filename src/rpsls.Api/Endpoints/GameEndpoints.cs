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
        
        group.MapPost("/play", async (PlayRequest request, ISender mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(request.ToPlayCommand(), ct);
            return Results.Ok(result);
        })
        .WithDescription("Play a round against a computer opponent.")
        .WithName("PlayUserGame")
        .Accepts<PlayRequest>("application/json")
        .Produces<ResultDto>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
        
        group.MapGet("/scoreboard", async (ISender mediator, [FromQuery]string username, [FromQuery]int count = 10, CancellationToken ct = default) =>
        {
            if (string.IsNullOrWhiteSpace(username))
                return Results.BadRequest("User parameter is required.");
            
            var result = await mediator.Send(new ScoreboardQuery(username, count), ct);
            return Results.Ok(result);
        })
        .WithDescription("Get scoreboard of user's most recent results.")
        .WithName("GetScoreboard")
        .Produces<IEnumerable<ResultWithTimeDto>>()
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
        
        group.MapPost("/scoreboard/reset", async (ISender mediator) =>
        {
            await mediator.Send(new ResetScoreboardCommand());
            return Results.NoContent();
        })
        .WithDescription("Reset scoreboard.")
        .WithName("ResetScoreboard")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
        
        group.MapGet("/leaderboard", async (ISender mediator, [FromQuery]int count = 3, CancellationToken ct = default) =>
        {
            var result = await mediator.Send(new LeaderboardQuery(count), ct);
            return Results.Ok(result);
        })
        .WithDescription("Get leaderboard of the top rated players.")
        .WithName("GetLeaderboard")
        .Produces<IEnumerable<LeaderboardEntryDto>>()
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}