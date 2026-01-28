using MassTransit;
using Shared.Contracts;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InventoryService
{
    public class OrderCreatedConsumer : IConsumer<IOrderCreated>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IOrderCreated> context)
        {
            _logger.LogInformation("Received OrderCreated Event: OrderId {OrderId}, Customer: {CustomerName}", 
                context.Message.OrderId, context.Message.CustomerName);
            
            // Simulate inventory update
            _logger.LogInformation("Inventory updated for Order {OrderId}", context.Message.OrderId);

            return Task.CompletedTask;
        }
    }
}
