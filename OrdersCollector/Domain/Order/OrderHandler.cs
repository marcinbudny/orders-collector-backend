using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc.Internal;
using OrdersCollector.Domain.Order.Commands.V1;
using OrdersCollector.Utils;

namespace OrdersCollector.Domain.Order
{
    public class OrderHandler
    {
        private readonly IRepository<Order> _repository;
        private readonly ITimeProvider _timeProvider;

        public OrderHandler(
            IRepository<Order> repository,
            ITimeProvider timeProvider) 
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }
        
        public async Task<string> Handle(OrderItem cmd)
        {
            var orderId = OrderId.From(cmd.LocalId, OrderDate.Today(_timeProvider));
            var order = await _repository.LoadById(orderId.Value) 
                        ?? new Order(orderId, cmd.LocalId, new OrderDate(_timeProvider.UtcNow.Date));

            order.OrderNewItem(cmd.ItemName, cmd.PersonName, addedAt: _timeProvider.UtcNow);

            await _repository.Save(order, cmd.CommandId);

            return orderId.Value;
        }
        
        public async Task Handle(RemoveItem command) => 
            await Update(command?.OrderId, order => order.RemoveItem(from: command?.PersonName), command?.CommandId);


        public async Task Handle(SelectResponsiblePerson command) => 
            await Update(command?.OrderId, order => order.SelectResponsiblePerson(), command?.CommandId);

        public async Task Update(string id, Action<Order> update, string commandId = null)
        {
            if(string.IsNullOrWhiteSpace(id))
                throw new DomainException($"Order id cannot be empty", ErrorCode.IdCannotBeEmpty);
            
            var order = await _repository.LoadById(id);
            if(order == null)
                throw new DomainException($"Order {id} not found", ErrorCode.NotFound);

            update(order);

            await _repository.Save(order, commandId);
        }
    }
}