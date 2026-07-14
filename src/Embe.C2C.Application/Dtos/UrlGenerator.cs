using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Application.Dtos
{
    public interface IFileUrlGenerator
    {
        Task<string?> GenerateUrlAsync(string fileName, FilePermissions filePermissions, CancellationToken cancellationToken = default);
    }

    public class FileUrlGenerator : IFileUrlGenerator
    {
        private readonly IImageService _fileService;
        private readonly TimeSpan _sasDuration;

        public FileUrlGenerator
        (
            IImageService fileService, 
            TimeSpan sasDuration
        )
        {
            _fileService = fileService;
            _sasDuration = sasDuration;
        }

        public Task<string?> GenerateUrlAsync(string fileName, FilePermissions filePermissions, CancellationToken cancellationToken = default)
        {
            return _fileService.GenerateImageSasUrlAsync(fileName, Domain.ValueObjects.ImageStatus.Accepted, filePermissions, _sasDuration, cancellationToken);

        }
    }
}