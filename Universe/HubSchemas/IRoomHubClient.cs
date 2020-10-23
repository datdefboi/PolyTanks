using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Core.HubSchemas
{
    public interface IRoomHubClient
    {
        public Task UpdateTanks(TankState own, TankState[] others);
        public Task LoadMap(string name);
    }

}