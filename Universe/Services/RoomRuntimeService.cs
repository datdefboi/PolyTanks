﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Core;
using Core.Abstractions;
using Core.HubSchemas;
using Core.Specs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PolyTanks.Helpers;
using Timer = System.Timers.Timer;

namespace Server.Services
{
    public class RoomRuntimeService
    {
        private readonly IHubContext<RoomHub, IRoomHubClient> _room;
        private readonly ILogger<RoomRuntimeService> _logger;

        private const float TickTime = 1 / 3f;

        private Timer timer;
        private DateTime lastUpdate;

        private Dictionary<string, PlayerData> _playerDatas = new Dictionary<string, PlayerData>();

        public RoomRuntimeService(IHubContext<RoomHub, IRoomHubClient> room, ILogger<RoomRuntimeService> logger)
        {
            _room = room;
            _logger = logger;
        }

        private TankState CreateNewTank()
        {
            var go = new TankState();
            go.Position = new Vector(0, 0);

            return go;
        }

        public void AddNewPlayer(string pID)
        {
            var newTank = CreateNewTank();

            Console.WriteLine("New player in room");

            _playerDatas.Add(pID, new PlayerData
            {
                keys = new List<string>(),
                tank = newTank
            });
        }

        public void RemovePlayer(string pID)
        {
        }

        public void HandleKeyDown(string pID, string key)
        {
            _playerDatas[pID].keys.Add(key);
        }

        public void HandleMouseDirection(string pID, float dir)
        {
            _playerDatas[pID].mouseDir = dir;
        }

        public void HandleKeyUp(string pID, string key)
        {
            _playerDatas[pID].keys.RemoveAll(p => p == key);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(TickTime);
            timer.Elapsed += TimerOnElapsed;
            timer.Start();
            lastUpdate = DateTime.Now;
        }

        private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _logger.LogTrace("Universe tick");

            foreach (var (id, data) in _playerDatas)
            {
                var appliance = AppliancesRepository.ForID(data.tank.ApplianceID);
                TankController.Update(data.tank, appliance, data,
                    (float) (DateTime.Now - lastUpdate).TotalSeconds);

                await _room.Clients.Client(id)
                    .UpdateTanks(data.tank, _playerDatas
                        .Where(p => p.Key != id)
                        .Select(p => p.Value.tank)
                        .ToArray());
            }

            lastUpdate = DateTime.Now;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Stop();
        }
    }
}