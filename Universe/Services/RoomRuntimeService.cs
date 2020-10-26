using System;
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
using PolyTanks.Shared;
using PolyTanks.Shared.Maps;
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

        private MapBase _map;

        private Dictionary<string, PlayerData> _playerDatas = new Dictionary<string, PlayerData>();

        public RoomRuntimeService(IHubContext<RoomHub, IRoomHubClient> room, ILogger<RoomRuntimeService> logger)
        {
            _room = room;

            _map = new BerlinMap();

            _logger = logger;
        }

        private TankState CreateNewTank()
        {
            var go = new TankState();
            go.Position = new Vector(0, 0);

            return go;
        }

        public async Task AddNewPlayerAsync(string pID)
        {
            var newTank = CreateNewTank();

            Console.WriteLine("New player in room");

            await _room.Clients.Client(pID).LoadMap("Berlin");

            var availablePoint = Enumerable.Range(0, _map.StartingPoints.Length);
            var n = availablePoint.Except(_playerDatas.Values.Select(p => p.spawnPointNumber)).First();

            newTank.Position = _map.StartingPoints[n] * _map.ScallingFactor;

            _playerDatas.Add(pID, new PlayerData
            {
                keys = new List<string>(),
                spawnPointNumber = n,
                team = n % 2,
                tank = newTank
            });
        }

        public async Task RemovePlayerAsync(string pID)
        {
            _playerDatas.Remove(pID);
            await UpdateUniverseAsync();
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
            await UpdateUniverseAsync();
        }

        private Vector GetIntersectionPoint(((Vector p1, Vector p2) own,
            (Vector p3, Vector p4) other) group)
        {
            var (p1, p2) = group.own;
            var (p3, p4) = group.other;

            if (p1.X >= p2.X)
            {
                var t = p1;
                p1 = p2;
                p2 = t;
            }

            if (p3.X >= p4.X)
            {
                var t = p3;
                p3 = p4;
                p4 = t;
            }

            float k1 = 0;
            if (p2.Y == p1.Y)
                k1 = 0;
            else
            {
                k1 = (p2.Y - p1.Y) / (p2.X - p1.X);
            }

            float k2 = 0;
            if (p4.Y == p3.Y)
                k2 = 0;
            else
            {
                k2 = (p4.Y - p3.Y) / (p4.X - p3.X);
            }

            float b1 = p1.Y - k1 * p1.X;
            float b2 = p3.Y - k2 * p3.X;

            float x = (b2 - b1) / (k1 - k2);
            float y = k1 * x + b1;

            return new Vector(x, y);
        }

        private PlayerData? CheckHit(string currentId, Vector from, float direction)
        {
            var totalHist = new List<(Vector pos, PlayerData? pd)>();

            var ray = new VectorGroup(from, from + Vector.FromAngle(direction - 90f) * 2000f);
            foreach (var pd in _playerDatas
                .Where(p => p.Key != currentId)
                .Select(p => p.Value))
            {
                var pdAppliance = AppliancesRepository.ForID(pd.tank.ApplianceID);
                var intersections = ray.FindIntersections(pdAppliance.Bounds
                    .Rotate(pdAppliance.Origin, pd.tank.Rotation)
                    .Move(pd.tank.Position));

                foreach (var intersection in intersections)
                {
                    var p = GetIntersectionPoint(intersection);
                    totalHist.Add((p, pd));
                }
            }

            totalHist.Sort((a, b) => from.DistaceTo(a.pos).CompareTo(from.DistaceTo(b.pos)));
            if (totalHist.Count == 0)
                return default;

            return totalHist.First().pd;
        }

        private async Task UpdateUniverseAsync()
        {
            var elapsed = (float) (DateTime.Now - lastUpdate).TotalSeconds;

            foreach (var (id, data) in _playerDatas)
            {
                var appliance = AppliancesRepository.ForID(data.tank.ApplianceID);
                TankController.Update(data.tank, appliance, data, elapsed);

                var enemies = _playerDatas
                    .Where(p => p.Key != id)
                    .Select(p => p.Value.tank)
                    .ToArray();

                foreach (var wall in _map.Walls)
                {
                    var opposBounds =
                        wall.Bounds
                            .Move(wall.Position)
                            .Scale(_map.ScallingFactor);

                    TankController.HandleCollisions(data.tank, appliance, opposBounds, elapsed);
                }

                foreach (var e in enemies)
                {
                    var oApp = AppliancesRepository.ForID(e.ApplianceID);
                    var bounds = oApp.Bounds
                        .Rotate(oApp.Origin, e.Rotation)
                        .Move(e.Position);

                    TankController.HandleCollisions(data.tank, appliance, bounds, elapsed);
                }

                if (data.keys.Contains("Space") && data.tank.Loading >= 1)
                {
                    data.tank.Loading = 0;
                    var target = CheckHit(id, data.tank.Position, data.tank.GunRotation);
                    if (target != default)
                    {
                        target.tank.Position = _map.StartingPoints[target.spawnPointNumber] * _map.ScallingFactor;
                        data.tank.Score++;
                        data.tank.Speed = 0;
                    }
                }

                await _room.Clients.Client(id)
                    .UpdateTanks(data.tank, enemies);
            }

            lastUpdate = DateTime.Now;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Stop();
        }
    }
}