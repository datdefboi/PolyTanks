using System.Collections.Generic;
using Core;
using PolyTanks.Shared;

namespace Server
{
    public class PlayerData
    {
        public List<string> keys;
        public int team;
        public int spawnPointNumber;
        public float mouseDir;
        public TankState tank;
    }
}