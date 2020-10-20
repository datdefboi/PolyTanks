using PolyTanks.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client;
using Core;
using Core.Specs;
using Microsoft.AspNetCore.SignalR.Client;
using PolyTanks.Engine;
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

        private async void RenderForm_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;

            await StartGame();
        }

        private async Task StartGame()
        {
            roomHub = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/room")
                .Build();

            roomHub.On<TankState, IEnumerable<TankState>>("UpdateTanks", TankUpdates);

            await roomHub.StartAsync();
        }

        private void TankUpdates(TankState cs, IEnumerable<TankState> ens)
        {
            currentState = cs;
            Refresh();
        }

        private void OnWorldPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            _frame = new Frame(g, Width, Height);

            var rq = new List<TankState>();
            rq.Add(currentState);
            rq.AddRange(enemiesStates);

            foreach (var state in rq)
            {
                if (state != default)
                {
                    _t1Appliance.Render(_frame, state);
                }
            }

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
                var x = e.Location.X / (float)Width * 2 - 1;
                var y = e.Location.Y / (float)Height * 2 - 1;
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