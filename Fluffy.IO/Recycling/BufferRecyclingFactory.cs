﻿using Fluffy.IO.Buffer;

namespace Fluffy.IO.Recycling
{
    public class BufferRecyclingFactory<T> : IObjectRecyclingFactory<T>, ICapacity//, IRecycler<FluffyBuffer>
        where T : FluffyBuffer, IResettable, ICapacityInitiatable, new()
    {
        private readonly ByteArrayRecycler _byteArrayRecycler;

        public int Capacity { get; }

        public BufferRecyclingFactory(int bufferSize)
            : this(new ByteArrayRecycler(bufferSize))
        {
        }

        internal BufferRecyclingFactory(ByteArrayRecycler byteArrayRecycler)
        {
            Capacity = byteArrayRecycler.Capacity;
            _byteArrayRecycler = byteArrayRecycler;
        }

        public T Get()
        {
            var result = new T();
            result.Initialize(_byteArrayRecycler);
            result.Initialize(_byteArrayRecycler.Get());
            return result;
        }

        public void Recycle(T @object)
        {
            Recycle(@object);
            //@object.Reset();
            //_bufferStack.TryAdd(@object);
        }

        public void Recycle(FluffyBuffer @object)
        {
            _byteArrayRecycler.Recycle(@object.Value);
            @object.Dispose();
            // Recycle((T)@object);
        }
    }
}