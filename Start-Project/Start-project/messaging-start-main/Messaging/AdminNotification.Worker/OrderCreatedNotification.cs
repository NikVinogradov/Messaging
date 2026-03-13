using Contracts.Events;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace AdminNotification.Worker
{
    public class OrderCreatedNotification : IConsumer<OrderCreated>
    {
        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            await Task.Delay(100);
            Console.WriteLine($"Admin Notification - I just consumed a message with orderId {context.Message.OrderId}, that was created at {context.Message.CreatedAt}");
        }
    }
}
