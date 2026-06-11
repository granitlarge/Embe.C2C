using Embe.C2C.Application.Abstractions.Services;

namespace Embe.C2C.Application.Dtos
{
    public interface IFileUrlGenerator
    {
        Task<string> GenerateUrlAsync(string fileName);
    }

    public class FileUrlGenerator : IFileUrlGenerator
    {
        private readonly IFileService _fileService;
        private readonly TimeSpan _sasDuration;

        public FileUrlGenerator(IFileService fileService, TimeSpan sasDuration)
        {
            _fileService = fileService;
            _sasDuration = sasDuration;
        }

        public Task<string> GenerateUrlAsync(string fileName)
        {
            return _fileService.GenerateFileSasUrlAsync(fileName, _sasDuration);
        }
    }
}