using System.Net;
using Carter;
using Carter.Response;
using OrdersCollector.Domain.Order;
using OrdersCollector.Domain.Order.Commands.V1;
using Serilog;
using static OrdersCollector.Api.ModuleCommon;

namespace OrdersCollector.Api.Order
{
    public class OrderModule : CarterModule
    {
        private static readonly ILogger Logger = Log.ForContext<OrderModule>();
        
        public OrderModule(OrderHandler handler)
        {
            Post("/orders/order-item", async ctx =>
            {
                await RunWithErrorHandling<OrderItem>(ctx, 
                    async cmd =>
                    {
                        var orderId = await handler.Handle(cmd);

                        ctx.Response.StatusCode = (int) HttpStatusCode.Created;
                        ctx.Response.Headers.Add("Location", GetLocationHeader(ctx.Request, $"orders/{orderId}"));
                        await ctx.Response.Negotiate(new { OrderId = orderId});
                    });
            });
            
            Post("/orders/select-responsible-person", async ctx =>
            {
                await RunWithErrorHandling<SelectResponsiblePerson>(ctx, 
                    async cmd =>
                    {
                        await handler.Handle(cmd);

                        ctx.Response.StatusCode = (int) HttpStatusCode.Accepted;
                    });
            });
            
            Post("/orders/remove-item", ctx => RunWithErrorHandlingAndAccept<RemoveItem>(ctx, handler.Handle));
        }
    }
}