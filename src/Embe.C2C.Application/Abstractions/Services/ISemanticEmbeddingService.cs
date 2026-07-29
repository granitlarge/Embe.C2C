namespace Embe.C2C.Application.Abstractions.Services;

public interface ISemanticEmbeddingService
{
    Task<float[]> GetAsync(string content, CancellationToken cancellationToken);
}