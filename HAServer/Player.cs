using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BepuPhysics;
using HAServer;
using System.Security.Cryptography.X509Certificates;
namespace GameServer.Project
{
    class Player
    {
       
        public ushort id;
        public string username;
        public Vector3 position;
        public Quaternion rotation;
        private bool[] inputs;
        private Quaternion camQuaternion;

        private float moveSpeed = 15f / Constants.TICKS_PER_SEC;

        public Player(ushort _id, string _username, Vector3 _spawnPosition)
        {
            id = _id;
            username = _username;
            position = _spawnPosition;
            rotation = Quaternion.Identity;
            inputs = new bool[4];
        }


        public void Update()
        {
            Vector2 _inputDirection = Vector2.Zero;
            if (inputs[0])
            {
                _inputDirection.Y += 1;
            }
            if (inputs[1])
            {
                _inputDirection.Y -= 1;
            }
            if (inputs[2])
            {
                _inputDirection.X -= 1;
            }
            if (inputs[3])
            {
                _inputDirection.X += 1;
            }

            Move(_inputDirection);
        }

        private void Move(Vector2 Movedirection)
        {
            Vector2 direction = Movedirection.LengthSquared() > 0 ? Vector2.Normalize(Movedirection) : Vector2.Zero;
            position += InputToWorldDirect(Movedirection) * moveSpeed;
            SendMovement();
        }
        private Vector3 InputToWorldDirect(Vector2 Input)
        {
            Vector3 rightDir = RotateVectorByQuaternion(Vector3.UnitX, camQuaternion);
            Vector3 upDir = Vector3.Cross(rightDir, Vector3.UnitY);
            Vector3 worldDir = Input.X * rightDir + upDir * Input.Y;
            return new Vector3(worldDir.X,0,worldDir.Z);
        }
        private Vector3 RotateVectorByQuaternion(Vector3 vector, Quaternion quaternion)
        {
            // Công thức: v' = q * v * q^-1
            Quaternion vectorQuat = new Quaternion(vector.X, vector.Y, vector.Z, 0);
            Quaternion conjugate = Quaternion.Conjugate(quaternion); // q^-1
            Quaternion result = quaternion * vectorQuat * conjugate;
            return new Vector3(result.X, result.Y, result.Z);
        }
        private void SendMovement()
        {
            Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.playerMovement);
            message.AddUShort(id);
            message.AddVector3(position);
            ServerGame.Instance.SendToAll(message);
        }
        public void Destroy()
        {
            // remove các tài nguyên được khởi tạo như simulator của player 
            HandlePlayer.Players.Remove(id);
        }
        public void SetInput(bool[] _inputs,Quaternion camera)
        {
            inputs = _inputs;
            camQuaternion = camera;
        }
        public void SendSpawned()
        {
            ServerGame.Instance.SendToAll(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawned)));
        }

        public void SendSpawned(ushort toClientId)
        {
            ServerGame.Instance.Send(AddSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.playerSpawned)), toClientId);
        }
        private Message AddSpawnData(Message message)
        {
            message.AddUShort(id);
            message.AddString(username);
            message.AddVector3(position);
            return message;
        }
     
    }
}
