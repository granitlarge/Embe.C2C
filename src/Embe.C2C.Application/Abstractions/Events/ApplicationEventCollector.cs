namespace Embe.C2C.Application.Abstractions.Events;

public abstract class ApplicationEventCollector
{
    private readonly List<ApplicationEvent> _collectedEvents = [];
    public IReadOnlyList<ApplicationEvent> CollectedEvents => _collectedEvents;

    protected void AddApplicationEvent(ApplicationEvent applicationEvent)
    {
        _collectedEvents.Add(applicationEvent);
    }

    public void ClearApplicationEvents()
    {
        _collectedEvents.Clear();
    }
}