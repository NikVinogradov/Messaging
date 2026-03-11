namespace Contracts.Events
{
    public class OrderReceived
    {

        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
