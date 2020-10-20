using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.HubSchemas;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Server.Services;
using Timer = System.Threading.Timer;

namespace Server
{
    public class RoomHub : Hub<IRoomHubClient>
    {
        private readonly ILogger<RoomHub> _logger;
        private readonly RoomRuntimeService _roomRuntime;

        public RoomHub(ILogger<RoomHub> logger, RoomRuntimeService roomRuntime)
        {
            _logger = logger;
            _roomRuntime = roomRuntime;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("New player in room");

            await _roomRuntime.AddNewPlayerAsync(Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Player disconnected");

            await _roomRuntime.RemovePlayerAsync(Context.ConnectionId);
        }

        public async Task KeyDown(string key)
        {
            _logger.LogInformation($"User input down {Context.ConnectionId}");

            _roomRuntime.HandleKeyDown(Context.ConnectionId, key);
        }

        public async Task MouseMove(float dir)
        {
            _logger.LogInformation($"User input mouse {Context.ConnectionId} {dir}");

            _roomRuntime.HandleMouseDirection(Context.ConnectionId, dir);
        }

        public async Task KeyUp(string key)
        {
            _logger.LogInformation($"User input up {Context.ConnectionId}");

            _roomRuntime.HandleKeyUp(Context.ConnectionId, key);
        }
    }
}