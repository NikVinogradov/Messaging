using MassTransit;

namespace Contracts.Events
{
    [EntityName("OrderCreated")]
    public class OrderCreated
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Decimal TotalAmount { get; set; }
    }
}
