﻿using Fluffy.IO.Buffer;
using Fluffy.Net.Options;
using Fluffy.Unsafe;

using System;

namespace Fluffy.Net.Packets.Modules
{
    internal class StandardOutputPacket : IOutputPacket
    {
        private readonly LinkedStream _stream;
        private bool _isDisposed;

        public byte OpCode { get; set; }
        public ParallelismOptions ParallelismOptions { get; set; }

        /// <summary>
        /// Is True when send progress has finished
        /// </summary>
        public bool HasFinished { get; private set; }

        public bool IsPrioritized { get; set; }

        /// <summary>
        /// Defines wether the handler can switch to more important Packets safely
        /// </summary>
        public bool CanBreak { get; private set; }

        public bool HasSendHeaders { get; private set; }

        public StandardOutputPacket(byte opCode, ParallelismOptions parallelismOption, LinkedStream stream)
        {
            IsPrioritized = true;
            OpCode = opCode;
            ParallelismOptions = parallelismOption;
            _stream = stream;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (count <= 0)
            {
                return 0;
            }

            if (count > buffer.Length - offset - 6)
            {
                count = buffer.Length - offset - 6;
            }

            if (_stream?.Length < 0)
            {
                throw new AggregateException("Stream length cannot be less than 0");
            }

            if (_stream == null || _stream.Length == 0)
            {
                HasFinished = true;
                CanBreak = true;
                Dispose();
                return 0;
            }

            int read = 0;
            if (!HasSendHeaders)
            {
                var header = new PacketHeader
                {
                    PacketLength = (int)_stream.Length + 2,
                    ParallelismOptions = (byte)ParallelismOptions,
                    OpCode = this.OpCode,
                };

                offset += read = FluffyBitConverter.Serialize(header, buffer, offset);


                HasSendHeaders = true;
            }

            read += _stream.Read(buffer, offset, count);
            if (_stream == null || _stream.Length == 0)
            {
                HasFinished = true;
                CanBreak = true;
                Dispose();
            }

            return read;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            _stream?.Dispose();
        }
    }
}