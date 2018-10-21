using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace OrdersCollector.Publishing
{
    public class EventsHub : Hub
    {
        private static ILogger Logger = Log.ForContext<EventsHub>();
        
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("welcome", "hello mate");
            
            Logger.Debug($"New connection {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Logger.Debug($"Disconnected {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }    
    }
}