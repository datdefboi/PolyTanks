using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Specs;
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

            var lastRotation = state.Rotation;

            if (data.keys.Contains("S"))
            {
                state.Speed = (float) (Math.Max(0, state.Speed - 40 * elapsed));
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

        public static bool CheckIntersections(TankState o1, TankAppliance appliance, MapBase map)
        {
            var selfBounds = s1.Bounds
                .Move(s1.Origin)
                .Rotate(Vector.Zero, o1.Rotation)
                .Move(o1.Position);

            var opposBounds =
                s2.Bounds
                    .Move(s2.Origin)
                    .Rotate(Vector.Zero, o2.Rotation)
                    .Move(o2.Position);

            if (o1.Position.DistaceTo(o2.Position) < s1.BoundsRadius + s2.BoundsRadius)
            {
                var isInters = selfBounds.IsIntersectsByBounding(opposBounds);

                if (isInters)
                {
                    if (o2 is IIntersectable)
                        ((IIntersectable) o2).OnIntersection(o1);

                    if (this is IIntersectable)
                        ((IIntersectable) this).OnIntersection(o2);

                    return true;
                }
            }

            return false;
        }
    }
}