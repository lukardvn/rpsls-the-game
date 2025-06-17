namespace rpsls.Application.Interfaces;

public interface IRandomNumberProvider
{
    Task<int> GetRandomNumber(CancellationToken ct = default);
}