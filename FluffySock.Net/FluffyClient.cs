﻿using Fluffy.IO.Buffer;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Fluffy.Net.Packets.Modules.Formatted;
using Fluffy.Net.Packets.Raw;

namespace Fluffy.Net
{
    public class FluffyClient : FluffySocket
    {
        private ConnectionInfo _connection;
        private IPEndPoint _endPoint;

        public FluffyClient(IPAddress address, int port)
        {
            _endPoint = new IPEndPoint(address, port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                Blocking = false,
                ReceiveTimeout = int.MaxValue,
                SendTimeout = int.MaxValue,
            };

            _connection = new ConnectionInfo(Socket, this);

            // _connection.TypedPacketHandler.
        }

        public void Connect()
        {
            Socket.Blocking = true;
            Socket.Connect(_endPoint);
            Socket.Blocking = false;

            _connection.Receiver.Start();
        }

        public Task ConnectAsync()
        {
            return Socket.ConnectAsync(_endPoint);
        }

        public void Test()
        {
            var str = new LinkedStream();
            var writeBuf = Encoding.UTF8.GetBytes("Hello World");
            str.Write(writeBuf, 0, writeBuf.Length);
            _connection.Sender.Send(PacketTypes.TestPacket, str);
        }

        public void TypedTest()
        {
            var obj = new MyAwesomeClass
            {
                AwesomeString = "AWESOME!!"
            };

            _connection.Sender.Send(obj);
        }
    }
}