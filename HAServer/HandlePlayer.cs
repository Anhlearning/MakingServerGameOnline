using GameServer.Project;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HAServer
{
    class HandlePlayer
    {
        public static Dictionary<ushort, Player> Players = new Dictionary<ushort, Player>();

        
        private static void HandleSendName(ushort clientId, Message message)
        {
            foreach (var otherPlayes in Players.Values)
            {
                otherPlayes.SendSpawned(clientId);
            }

            Player player = new Player(clientId, message.GetString(), new Vector3(0, 0, 0));
            Console.WriteLine(player.id +" " + player.username  + " Hello");
            player.SendSpawned();
            Players.Add(player.id, player);
        }

        [MessageHandler((ushort)ClientToServerId.name)]
        private static void Name(ushort clientId, Message message)
        {
            HandleSendName(clientId, message);
        }
        [MessageHandler((ushort) ClientToServerId.input)]
        private static void Input(ushort clientId,Message message)
        {
            if(Players.TryGetValue(clientId, out Player player))
            {
                player.SetInput(message.GetBools(4), message.GetQuaternion());
            }
        }
    }
}
