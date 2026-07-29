using Embe.C2C.Application.Abstractions.Services;
using OpenAI.Embeddings;

namespace Embe.C2C.Infrastructure.OpenAI;

public class OpenAISemanticEmbeddingService(Settings settings) : ISemanticEmbeddingService
{
    private readonly EmbeddingClient _embeddingClient = new("text-embedding-3-small", settings.OpenAISemanticEmbedding.ApiKey);

    public async Task<float[]> GetAsync(string content, CancellationToken cancellationToken)
    {
        var embedding = await _embeddingClient.GenerateEmbeddingAsync(content, cancellationToken: cancellationToken);
        return embedding.Value.ToFloats().ToArray();
    }
}

public class NullSemanticEmbeddingService : ISemanticEmbeddingService
{
    private static readonly float[] _nullEmbedding = new float[1536];

    public Task<float[]> GetAsync(string content, CancellationToken cancellationToken)
    {
        return Task.FromResult(_nullEmbedding);
    }
}