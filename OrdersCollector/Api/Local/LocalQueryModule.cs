using Carter;
using Carter.Response;
using Marten;

namespace OrdersCollector.Api.Local
{
    public class LocalQueryModule : CarterModule
    {
        public LocalQueryModule(IDocumentStore store)
        {
             Get("/locals", async ctx =>
             {
                 using (var session = store.QuerySession())
                 {
                     var locals = await session
                         .Query<Reads.Document.Local>()
                         .ToListAsync();

                     await ctx.Response.Negotiate(locals);
                 }
             });
        }    
    }
}