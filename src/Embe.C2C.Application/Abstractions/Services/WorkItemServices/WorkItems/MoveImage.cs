namespace Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;

public record MoveFile(string FromUrl, string ToUrl) : IWorkItem;