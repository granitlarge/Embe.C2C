using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Dtos
{
    public interface IFileUrlGenerator
    {
        Task<string?> GenerateUrlAsync(string fileName, ImageStatus imageStatus, ImageSize size, FilePermissions filePermissions, CancellationToken cancellationToken = default);
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

        public Task<string?> GenerateUrlAsync(string fileName, ImageStatus imageStatus, ImageSize size, FilePermissions filePermissions, CancellationToken cancellationToken = default)
        {
            return _fileService.GenerateImageSasUrlAsync(fileName, imageStatus, size, filePermissions, _sasDuration, cancellationToken);

        }
    }

}