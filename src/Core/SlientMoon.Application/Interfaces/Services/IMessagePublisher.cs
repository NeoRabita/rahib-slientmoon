using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string routingKey) where T : class;
    }
}
