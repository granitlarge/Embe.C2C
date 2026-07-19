namespace Embe.C2C.Application.Commands.Users;

public record AddImageCommand(string MimeType, int Order, Crop Crop);
public record Crop(double X, double Y, int Width, int Height);