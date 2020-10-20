using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Specs;
using Microsoft.Extensions.Logging;
using PolyTanks.Helpers;
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
                state.Speed = (float) (Math.Max(0, state.Speed - 40 * elapsed));
            }
            
            if (data.keys.Contains("A"))
            {
                state.Rotation += 30f * elapsed;
            }
            
            if (data.keys.Contains("D"))
            {
                state.Rotation -= 30f * elapsed;
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
                state.GunRotation = MathF.Reach(state.GunRotation, target, 30 * elapsed);

            /*if (Keyboard.Pressed[Keys.Up] && Speed < MaxSpeed)
                 Speed += Acceleration * d;
             if (Keyboard.Pressed[Keys.Down] && Speed > 0)
                 Speed -= Acceleration * d;
             Debug.Write("DBG:Speed KM/h", (Speed / AxisDistance * 4.4f) / 3.6 * 2);
 
             if (Keyboard.Pressed[Keys.Left])
                 SteerAngle += SteerSpeed * d;
             else if (Keyboard.Pressed[Keys.Right])
                 SteerAngle -= SteerSpeed * d;
             else if (Abs(SteerAngle) > 0.1f)
                 SteerAngle -= Sign(SteerAngle) * 40f * d;
 
             Debug.Write("DBG:steer angle", SteerAngle);
 
             var turnRadius = AxisDistance / Sin(SteerAngle);
             Debug.Write("DBG:radius", turnRadius);
             var currentRotation = ToDeg(Speed / turnRadius);
             overload = Speed / turnRadius;
             if (Math.Abs(overload) > 1)
             {
                 var angle = ToDeg((float) Math.Asin(
                     AxisDistance / Speed /
                     Math.Abs(overload)));
                 SteerAngle = Sign(SteerAngle) * angle;
             }
 
             SteerAngle = Sign(SteerAngle) * Min(MaxSteerAngle, Abs(SteerAngle));
 
             Debug.Write("DBG:ovd", overload);
             Debug.Write("DBG:cur rot", currentRotation);
             Rotate(currentRotation * d);
             //speed -= steerAngle * angularDrag * d;
 
             Fuel -= FuelConsumptionPerSec * d;
 
             if (Fuel < 0)
             {
                 Fuel = 0;
                 Destroy();
             }
 
             Position += Vector.FromAngle(Rotation) * Speed * d;*/
        }

        /*public static bool CheckIntersections(TankState o1, TankState o2)
        {
            var s1 = SpecsProvider.GetSpecForID(o1.SpecID);
            var s2 = SpecsProvider.GetSpecForID(o2.SpecID);
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
        }*/
    }
}