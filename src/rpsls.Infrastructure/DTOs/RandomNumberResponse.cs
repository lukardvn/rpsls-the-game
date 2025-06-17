using System.Text.Json.Serialization;

namespace rpsls.Infrastructure.DTOs;

public record RandomNumberResponse(
    [property: JsonPropertyName("random_number")] 
    int RandomNumber
);