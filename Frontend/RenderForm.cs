﻿using PolyTanks.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client;
using Core;
using Core.Specs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using PolyTanks.Engine;
using PolyTanks.Shared;
using PolyTanks.Shared.Maps;
using MathF = PolyTanks.Helpers.MathF;

namespace PolyTanks.Frontend
{
    public partial class RenderForm : Form
    {
        public RenderForm()
        {
            InitializeComponent();
        }

        private HubConnection roomHub;
        private TankState currentState;
        private IEnumerable<TankState> enemiesStates = new TankState[] { };

        private T1Appliance _t1Appliance = new T1Appliance();
        private IFrame _frame;

        private MapBase _map;

        private bool _isConnected = false;

        private async void RenderForm_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;

            await StartGame();
        }

        private async Task StartGame()
        {
            roomHub = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/room")
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.NumberHandling =
                        JsonNumberHandling.AllowNamedFloatingPointLiterals;
                })
                .Build();

            roomHub.On<TankState, IEnumerable<TankState>>("UpdateTanks", TankUpdates);
            roomHub.On<string>("LoadMap", LoadMap);

            await roomHub.StartAsync();
        }

        private void LoadMap(string name)
        {
            _map = name switch
            {
                "Berlin" => new BerlinMap()
            };
        }

        private void TankUpdates(TankState cs, IEnumerable<TankState> ens)
        {
            currentState = cs;
            enemiesStates = ens;
            _isConnected = true;
            Refresh();
        }

        private void OnWorldPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            _frame = new Frame(g, Width, Height, 0.5f);

            if (_isConnected)
            {
                _frame.LookAt(currentState.Position);

                var rq = new List<TankState>();
                rq.Add(currentState);
                rq.AddRange(enemiesStates);

                foreach (var state in rq)
                {
                    if (state != default)
                    {
                        _t1Appliance.Render(_frame, state);
                        var bounds = _t1Appliance.Bounds
                            .Rotate(_t1Appliance.Origin, state.Rotation)
                            .Move(state.Position);
                        _frame.DrawPolygon(bounds, Color.Lime);

                        var ray = new VectorGroup(state.Position,
                            state.Position + Vector.FromAngle(state.GunRotation - 90f) * 2000f);
                        if (state.Loading > 1)
                            _frame.DrawPolygon(ray, Color.Chocolate);
                    }
                }

                foreach (var wall in _map.Walls)
                {
                    _frame.FillPolygon(wall.Bounds.Move(wall.Position).Scale(_map.ScallingFactor),
                        Color.FromName(wall.ColorCode));

                    var bounds = wall.Bounds
                        .Move(wall.Position)
                        .Scale(_map.ScallingFactor);

                    _frame.DrawPolygon(bounds, Color.MediumPurple);
                }
                
                g.DrawString($"Score {currentState.Score}", new Font(FontFamily.GenericMonospace, 20), Brushes.Black, 0, 0);
            }
            else
            {
                g.DrawString("Connecting", new Font(FontFamily.GenericMonospace, 20), Brushes.Black, 0, 0);
            }

            if (currentState?.IsInters ?? false)
                g.DrawString("Oh no", new Font(FontFamily.GenericMonospace, 20), Brushes.PaleVioletRed, 0, 0);

            /*if (currentState != null)
                View.PlaceCamOn(currentState);*/
        }

        private void RenderTick(object sender, EventArgs e)
        {
        }

        private DateTime lastMouseEvent = DateTime.Now;
        private TimeSpan MouseCooldown = TimeSpan.FromMilliseconds(20);

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DateTime.Now - lastMouseEvent > MouseCooldown)
            {
                var x = e.Location.X / (float) Width * 2 - 1;
                var y = e.Location.Y / (float) Height * 2 - 1;
                var angle = MathF.Atan2(y, x);
                roomHub.SendAsync("MouseMove", angle);

                lastMouseEvent = DateTime.Now;
            }
        }

        private void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            roomHub.SendAsync("KeyDown", e.KeyCode.ToString());
        }

        private void RenderForm_KeyUp(object sender, KeyEventArgs e)
        {
            roomHub.SendAsync("KeyUp", e.KeyCode.ToString());
        }
    }
}