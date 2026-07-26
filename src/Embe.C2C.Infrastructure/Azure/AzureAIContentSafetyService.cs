using Azure;
using Azure.AI.ContentSafety;
using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Infrastructure.Azure;

public class NullContentSafetyService : IContentSafetyService
{
    public Task<decimal> GetSafetyScoreAsync(byte[] bytes, CancellationToken cancellationToken)
    {
        return Task.FromResult(0m);
    }
}

public class AzureAIContentSafetyService : IContentSafetyService
{
    private readonly ContentSafetyClient _contentSafetyClient;
    private readonly BlocklistClient _blocklistClient;

    public AzureAIContentSafetyService(Settings settings)
    {
        var url = settings.AzureAIContentSafety.Url;
        var apiKey = settings.AzureAIContentSafety.ApiKey;
        _contentSafetyClient = new ContentSafetyClient(new Uri(url), new AzureKeyCredential(apiKey));
        _blocklistClient = new BlocklistClient(new Uri(url), new AzureKeyCredential(apiKey));
    }

    public async Task<decimal> GetSafetyScoreAsync(byte[] bytes, CancellationToken cancellationToken)
    {
        var analyzeImageResult = await _contentSafetyClient.AnalyzeImageAsync(new AnalyzeImageOptions(new ContentSafetyImageData(new BinaryData(bytes))), cancellationToken);
        var maximumSeverityLevelAcrossAllCategories = analyzeImageResult.Value.CategoriesAnalysis.Max(ca => ca.Severity ?? 0);
        return (decimal)maximumSeverityLevelAcrossAllCategories / 6;
    }
}