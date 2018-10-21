using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Carter.ModelBinding;
using Carter.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OrdersCollector.Api.Contract;
using OrdersCollector.Domain;
using Serilog;
using Serilog.Core;

namespace OrdersCollector.Api
{
    public static class ModuleCommon
    {
        private static readonly ILogger Logger = Log.ForContext(Constants.SourceContextPropertyName, typeof(ModuleCommon).FullName);
        
        public static string GetLocationHeader(HttpRequest req, string path) => $"{req.Scheme}://{req.Host}/{path}";
        
        public static async Task RunWithErrorHandling<TCommand>(HttpContext ctx, Func<TCommand, Task> handle)
        {
            try
            {
                var cmd = ctx.Request.Bind<TCommand>();

                await handle(cmd);

            }
            catch (DomainException e)
            {
                Logger.Warning(e, "Domain error: {ErrorCore}", e.ErrorCode);
                ctx.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                await ctx.Response.Negotiate(Error.From(e));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Internal server error");
                ctx.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                await ctx.Response.Negotiate(Error.InternalServerError);
            }
        }
        
        public static Task RunWithErrorHandlingAndAccept<TCommand>(HttpContext ctx, Func<TCommand, Task> handle)
        {
            return RunWithErrorHandling<TCommand>(ctx, async cmd =>
            {
                await handle(cmd);
                ctx.Response.StatusCode = (int) HttpStatusCode.Accepted;
            });
        }
    }
}