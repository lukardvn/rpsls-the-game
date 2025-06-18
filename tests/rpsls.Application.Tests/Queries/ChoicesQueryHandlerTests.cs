using rpsls.Application.DTOs;
using rpsls.Application.Queries;
using rpsls.Domain.Models;

namespace rpsls.Application.Tests.Queries;

public class ChoicesQueryHandlerTests
{
    private readonly ChoicesQueryHandler _handler = new();

    [Fact]
    public async Task Handle_ShouldReturnAllEnumChoices()
    {
        // Arrange
        var query = new ChoicesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var expected = Enum.GetValues<Choice>()
            .Select(c => new ChoiceDto((int)c, c.ToString()))
            .ToList();

        var choiceDtos = result as ChoiceDto[] ?? result.ToArray();
        Assert.Equal(expected.Count, choiceDtos.Length);
        foreach (var dto in expected)
            Assert.Contains(choiceDtos, r => r.Id == dto.Id && r.Name == dto.Name);
    }
}