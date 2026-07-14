namespace Embe.C2C.Application.Abstractions.Services;

public interface IContentSafetyService
{
    /// <summary>
    /// returns a number between 0 & 1. The higher the number, the less safe the image is.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<decimal> GetSafetyScoreAsync(byte[] bytes, CancellationToken cancellationToken);
}