using System.Linq;
using System.Net;
using Carter;
using Carter.Request;
using Carter.Response;
using Marten;
using Microsoft.AspNetCore.Routing;
using OrdersCollector.Utils;

namespace OrdersCollector.Api.Order
{
    public class OrderQueryModule : CarterModule
    {
        public OrderQueryModule(IDocumentStore store, ITimeProvider timeProvider)
        {
            Get("orders/today", async ctx =>
            {
                var today = timeProvider.UtcNow.Date;
                
                using (var session = store.QuerySession())
                {
                    var orders = await session
                        .Query<Reads.Document.Order>()
                        .Where(o => o.Date == today)
                        .ToListAsync();
                    
                    await ctx.Response.Negotiate(orders);
                }
            });

            Get("orders/{orderId}", async ctx =>
            {
                var orderId = ctx.GetRouteData().As<string>("orderId");

                using (var session = store.QuerySession())
                {
                    var order = await session.LoadAsync<Reads.Document.Order>(orderId);
                    if (order == null)
                    {
                        ctx.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    }
                    else
                    {
                        await ctx.Response.Negotiate(order);
                    }
                }
            });
            

        }
    }
}