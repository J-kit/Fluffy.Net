﻿using Fluffy.Net;
using Fluffy.Net.Packets.Modules.Formatted;

using System;
using System.Diagnostics;
using System.Net;

namespace NetSocket
{
    internal class Program
    {
        private static FluffyServer _server;
        private static FluffyClient _client;

        private static void Main(string[] args)
        {
            var guid = Guid.NewGuid();

            var x = guid.ToByteArray();
            var ng = new Guid(x);

            _server = new FluffyServer(8090);
            _client = new FluffyClient(IPAddress.Loopback, 8090);

            _client.Connection.PacketHandler.On<MyAwesomeClass>().Do(Awesome);
            _server.PacketHandler.On<MyAwesomeClass>().Do(Awesome);
            _server.Start();
            _server.OnNewConnection += OnNewConnection;

            _client.Connect();
            Console.WriteLine("Connected");
            TypedTest(_client.Connection);
            //_client.Test();
            //_client.Test();
            Console.WriteLine("Test sent");
            Console.ReadLine();
        }

        private static void OnNewConnection(object sender, ConnectionInfo connection)
        {
            var awesome = new MyAwesomeClass
            {
                AwesomeString = "AWESOME!!"
            };

            _sw = Stopwatch.StartNew();
            while (true)
            {
                // awesome = srvEp.PacketHandler.Handle(awesome) as MyAwesomeClass;
                awesome = connection.Sender.Send<MyAwesomeClass>(awesome).Value;

                if (awesome.Packets % 300 == 0)
                {
                    _sw.Stop();
                    Console.WriteLine($"AVG Delay: {(_sw.Elapsed.TotalMilliseconds / awesome.Packets)} ms " +
                                      $"{awesome.Packets}:  ({awesome.Packets * 2 / _sw.Elapsed.TotalMilliseconds})");
                    _sw.Start();
                }
            }
        }

        private static Stopwatch _sw;

        private static void TypedTest(ConnectionInfo connection)
        {
        }

        private static MyAwesomeClass Awesome(MyAwesomeClass awesome)
        {
            awesome.Packets++;
            return awesome;
        }
    }
}