// using System.Net.Http.Json;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.VisualStudio.TestPlatform.TestHost;
// using MediatR;
// using Moq;
// using rpsls.Api.DTOs;
// using rpsls.Application.Commands;
// using rpsls.Application.DTOs;
// using rpsls.Application.Queries;
// using rpsls.Domain.Models;
//
// namespace rpsls.Api.Tests;
//
// public class GameEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
// {
//     private readonly WebApplicationFactory<Program> _factory;
//     private readonly Mock<ISender> _mediatorMock;
//
//     public GameEndpointsTests(WebApplicationFactory<Program> factory)
//     {
//         _mediatorMock = new Mock<ISender>();
//
//         _factory = factory.WithWebHostBuilder(builder =>
//         {
//             builder.ConfigureServices(services =>
//             {
//                 // Remove the existing ISender registration and add our mock
//                 var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISender));
//                 if (descriptor != null)
//                     services.Remove(descriptor);
//
//                 services.AddSingleton(_mediatorMock.Object);
//             });
//         });
//     }
//
//     [Fact]
//     public async Task GetChoices_ReturnsOkWithChoices()
//     {
//         // Arrange
//         var expectedChoices = new List<ChoiceDto>
//         {
//             new(1, "Rock"),
//             new(2, "Paper")
//         };
//
//         _mediatorMock
//             .Setup(m => m.Send(It.IsAny<ChoicesQuery>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(expectedChoices);
//
//         var client = _factory.CreateClient();
//
//         // Act
//         var response = await client.GetAsync("/game/choices");
//
//         // Assert
//         response.EnsureSuccessStatusCode();
//
//         var result = await response.Content.ReadFromJsonAsync<List<ChoiceDto>>();
//         Assert.NotNull(result);
//         Assert.Equal(expectedChoices.Count, result.Count);
//         Assert.Equal(expectedChoices[0].Name, result[0].Name);
//     }
//
//     [Fact]
//     public async Task PlayGame_ReturnsOkWithResult()
//     {
//         // Arrange
//         var playRequest = new UserPlayRequest((int)Choice.Rock, null);
//         var expectedResult = new ResultWithTimeDto((Choice)playRequest.Choice, Choice.Lizard, Outcome.Lose, DateTime.Today);
//
//         _mediatorMock
//             .Setup(m => m.Send(It.IsAny<UserPlayCommand>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(expectedResult);
//
//         var client = _factory.CreateClient();
//
//         // Act
//         var response = await client.PostAsJsonAsync("/game/play", playRequest);
//
//         // Assert
//         response.EnsureSuccessStatusCode();
//
//         var result = await response.Content.ReadFromJsonAsync<ResultDto>();
//         Assert.NotNull(result);
//         Assert.Equal(expectedResult.Result, result.Result);
//     }
// }
