using System.Threading.Tasks;

namespace Transaction.Aplication.Interfaces
{
    public interface IEventProducer
    {
        Task ProduceAsync<T>(string topic, T @event);
    }
}
