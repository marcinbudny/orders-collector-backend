using System.Net;
using Carter;
using Carter.Response;
using OrdersCollector.Domain.Local;
using OrdersCollector.Domain.Local.Commands.V1;
using static OrdersCollector.Api.ModuleCommon;

namespace OrdersCollector.Api.Local
{
    public class LocalModule : CarterModule
    {
        public LocalModule(LocalHandler handler)
        {
            Post("/locals/add-local", async ctx => { 
                await RunWithErrorHandling<AddLocal>(ctx, async cmd =>
                {
                    await handler.Handle(cmd);
                    
                    ctx.Response.StatusCode = (int) HttpStatusCode.Created;
                    ctx.Response.Headers.Add("Location", GetLocationHeader(ctx.Request, $"locals/{cmd.Id}"));
                }); 
            });

            Post("/locals/add-alias", async ctx => { 
                await RunWithErrorHandling<AddLocalAlias>(ctx, async cmd =>
                {
                    await handler.Handle(cmd);
                    
                    ctx.Response.StatusCode = (int) HttpStatusCode.Created;
                    ctx.Response.Headers.Add("Location", GetLocationHeader(ctx.Request, $"locals/{cmd.LocalId}"));
                }); 
            });

            Post("/locals/rename-local", ctx => RunWithErrorHandlingAndAccept<RenameLocal>(ctx, handler.Handle));

            Post("/locals/remove-local-alias", ctx => RunWithErrorHandlingAndAccept<RemoveLocalAlias>(ctx, handler.Handle));

            Post("/locals/remove-local", ctx => RunWithErrorHandlingAndAccept<RemoveLocal>(ctx, handler.Handle));
            
        }
    }
}