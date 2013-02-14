using Caliburn.Micro;

namespace MyCoolApp.Domain
{
    public class GlobalEventAggregator : EventAggregator
    {
         public static IEventAggregator Instance = new EventAggregator();
    }
}