using GameServer.Project;
using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAServer
{
    class ServerGame
    {
        private  Server server;
        private  ushort maxPlayer;
        private  ushort port;

        public  Server Server { get; private set; }

        public static ServerGame Instance { get; private set; }

        public ServerGame(ushort maxPlayer,ushort port)
        {
            RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);

            server = new Server();
            server.TimeoutTime = 1000;
            this.maxPlayer = maxPlayer;
            this.port = port;
            Server = server;
            server.Start(this.port, this.maxPlayer);
            Instance = this;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            //Server.ClientConnected += NewPlayerConnected;
            Server.ClientDisconnected += PlayerLeft;
        }

        private void PlayerLeft(object? sender, ServerDisconnectedEventArgs e)
        {
            if(HandlePlayer.Players.TryGetValue(e.Client.Id,out Player player))
            {
                player.Destroy();
            }
        }
        private void OnProcessExit(object? sender, EventArgs e)
        {
            Server.Stop();
        }
        private void NewPlayerConnected(object? sender, ServerConnectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void SendToAll(Message message)
        {
            Server.SendToAll(message);
        }
        public void Send(Message message,ushort clientId)
        {
            Server.Send(message, clientId);
        }


    }
}
