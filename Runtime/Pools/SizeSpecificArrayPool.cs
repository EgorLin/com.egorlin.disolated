using System;

namespace EgorLin.DIsolated.Pools
{
    internal sealed class SizeSpecificArrayPool<T> where T : new()
    {
        public const int InitialBucketSize = 4;
        
        private readonly T[][][] _buckets; // Similar to a Dictionary<int, Stack<T[]>> but all inlined for performance
        private readonly int[] _rentals;

        public SizeSpecificArrayPool(int maxLength)
        {
            maxLength += 1;

            _buckets = new T[maxLength][][];
            _rentals = new int[maxLength];

            for (var length = 0; length < maxLength; length++)
            {
                var bucket = new T[InitialBucketSize][];

                for (var j = 0; j < bucket.Length; j++)
                {
                    bucket[j] = new T[length];
                }

                _buckets[length] = bucket;
            }
        }

        public T[] Rent(int length)
        {
            var lengthSupportedMax = _buckets.Length - 1;
            if (length > lengthSupportedMax)
            {
                return new T[length];
            }

            var bucket = _buckets[length];
            var indexRental = _rentals[length];

            if (indexRental >= bucket.Length)
            {
                var newSize = bucket.Length * 2;
                Array.Resize(ref bucket, newSize);

                for (var i = indexRental; i < newSize; i++)
                {
                    bucket[i] = new T[length];
                }
            }

            var array = bucket[indexRental];
            _rentals[length]++;
            return array;
        }

        public void Return(T[] array)
        {
            var length = array.Length;
            var lengthSupportedMax = _buckets.Length - 1;

            if (length > lengthSupportedMax)
            {
                return;
            }

            Array.Clear(array, 0, length);
            _rentals[length]--;
        }
    }
}