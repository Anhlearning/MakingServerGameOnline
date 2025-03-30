using HAServer;
using Riptide;
using Riptide.Utils;
using System.Diagnostics;
namespace GameServer.Project
{
    public enum ServerToClientId : ushort
    {
        sync = 1,
        activeScene,
        playerSpawned,
        playerMovement,
        playerHealthChanged,
        playerActiveWeaponUpdated,
        playerAmmoChanged,
        playerDied,
        playerRespawned,
        projectileSpawned,
        projectileMovement,
        projectileCollided,
        projectileHitmarker,
    }

    public enum ClientToServerId : ushort
    {
        name = 1,
        input,
        switchActiveWeapon,
        primaryUse,
        reload,
    }
    internal class Program
    {

        static void Main(string[] args)
        {
            

            InitServer();
        }


       

        public static void InitServer()
        {
            ServerGame haServer = new ServerGame(2, 7777);

           

            DateTime _nextLoop = DateTime.Now; 
            while (true)
            {
                while (_nextLoop < DateTime.Now)
                {
                    haServer.Server.Update();
                    foreach (var player  in HandlePlayer.Players.Values)
                    {
                        if (player != null)
                        {
                            player.Update();
                        }
                    }
                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);
                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }

            }
        }

    }
}
