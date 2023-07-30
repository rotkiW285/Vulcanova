using System.Threading.Tasks;

namespace Vulcanova.Core.Uonet;

public interface IWidgetUpdater<in TEvent> where TEvent : UonetDataUpdatedEvent
{
    Task Handle(TEvent message);
}