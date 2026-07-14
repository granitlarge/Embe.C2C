using Embe.C2C.Application.Commands.Images;
using Embe.C2C.Application.Commands.Images.Handlers;
using Microsoft.Azure.Functions.Worker;

namespace Embe.C2C.Functions;

public class ImageProcessingPipelineFunctions
{
    private readonly ProcessUploadedImageHandler _processUploadedImageHandler;
    public ImageProcessingPipelineFunctions(ProcessUploadedImageHandler processUploadedImageHandler)
    {
        _processUploadedImageHandler = processUploadedImageHandler;
    }

    [Function(nameof(OnImageUploaded))]
    public async Task OnImageUploaded([BlobTrigger("images/pending/{name}")] Stream stream, string name)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        var command = new ProcessUploadedImageCommand(Guid.Parse(name), memoryStream.ToArray());
        var result = await _processUploadedImageHandler.HandleAsync(command);
    }
}