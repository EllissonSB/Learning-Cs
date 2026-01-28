using System;

namespace Shared.Contracts
{
    public interface IOrderCreated
    {
        Guid OrderId { get; }
        DateTime CreatedAt { get; }
        string CustomerName { get; }
        decimal TotalAmount { get; }
    }
}
