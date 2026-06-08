namespace Embe.C2C.Application.Events;

public abstract class IntegrationEventCollector
{
    private readonly List<IntegrationEvent> _collectedEvents = [];
    public IReadOnlyList<IntegrationEvent> CollectedEvents => _collectedEvents;

    protected void AddIntegrationEvent(IntegrationEvent integrationEvent)
    {
        _collectedEvents.Add(integrationEvent);
    }

    public void ClearIntegrationEvents()
    {
        _collectedEvents.Clear();
    }
}