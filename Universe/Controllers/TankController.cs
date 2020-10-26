using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Specs;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Logging;
using PolyTanks.Helpers;
using PolyTanks.Shared;
using Server;
using MathF = PolyTanks.Helpers.MathF;

namespace Core.Abstractions
{
    public static class TankController
    {
        public static void Update(TankState state, TankAppliance appliance, PlayerData data, float elapsed)
        {
            state.Position += Vector.FromAngle(state.Rotation) * state.Speed * elapsed;

            if (data.keys.Contains("W"))
            {
                state.Speed += (float) (appliance.Acceleration *
                                        Math.Cos(Math.PI * (state.Speed / appliance.MaxSpeed) / 2f) * elapsed);
            }

            if (data.keys.Contains("S"))
            {
                state.Speed = (float) PolyTanks.Helpers.MathF.Max(0, state.Speed - appliance.Acceleration * elapsed);
            }

            if (data.keys.Contains("A"))
            {
                var movement = appliance.RotationSpeed * elapsed;
                state.Rotation += movement;
                state.GunRotation += movement;
            }

            if (data.keys.Contains("D"))
            {
                var movement = appliance.RotationSpeed * elapsed;
                state.Rotation -= movement;
                state.GunRotation -= movement;
            }

            state.Loading += 0.5f * elapsed;

            //gun rotation
            var target = 0f;
            if (Math.Abs(data.mouseDir - state.GunRotation) > 180)
            {
                if (state.GunRotation > 0) target = 180;
                else if (state.GunRotation < 0) target = -180;
            }
            else
            {
                target = data.mouseDir;
            }

            if (state.GunRotation == 180 || state.GunRotation == -180)
                state.GunRotation = 179 * Math.Sign(data.mouseDir);
            else
                state.GunRotation = MathF.Reach(state.GunRotation, target, appliance.TurretSpeed * elapsed);
        }

        public static bool HandleCollisions(TankState state, TankAppliance appliance, VectorGroup oppos, float elapsed)
        {
            var selfBounds = appliance.Bounds
                .Rotate(appliance.Origin, state.Rotation)
                .Move(state.Position);

            var intersected = false;

            var interections = selfBounds.FindIntersections(oppos);

            foreach (var ((c, d), (a, b)) in interections)
            {
                intersected = true;

                var dir = state.Position.X * (a.Y - b.Y) * state.Position.Y * (b.X - a.X) + (a.X * b.Y - a.Y * b.X);

                var v = b - a;

                if (dir == 0) // sorry, that's magic
                    continue;

                var n = new Vector(v.Y, -v.X) * -(dir / MathF.Abs(dir)); // direction to throw body away

                state.Speed = 0;
                state.Position += n * 0.05f * elapsed;
            }


            return intersected;
        }
    }
}