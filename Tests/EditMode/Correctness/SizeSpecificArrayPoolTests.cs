using EgorLin.DIsolated.Pools;
using NUnit.Framework;

namespace EgorLin.DIsolated.Tests
{
    public class SizeSpecificArrayPoolTests
    {
        [Test]
        public void Rent_ReusesArrays_WhenFullyReturned_WithinInitialBucket()
        {
            var pool = new SizeSpecificArrayPool<object>(maxLength: 16);

            var first = new object[4][];
            for (var i = 0; i < first.Length; i++)
            {
                first[i] = pool.Rent(4);
            }

            for (var i = 0; i < first.Length; i++)
            {
                pool.Return(first[i]);
            }

            for (var i = 0; i < first.Length; i++)
            {
                Assert.AreSame(first[i], pool.Rent(4), $"Index {i} was not reused");
            }
        }

        [Test]
        public void Rent_ReusesArrays_WhenConcurrentExceedsInitialBucket()
        {
            var pool = new SizeSpecificArrayPool<object>(maxLength: 16);

            var first = new object[6][];
            for (var i = 0; i < first.Length; i++)
            {
                first[i] = pool.Rent(4);
            }

            for (var i = 0; i < first.Length; i++)
            {
                pool.Return(first[i]);
            }

            var second = new object[6][];
            for (var i = 0; i < second.Length; i++)
            {
                second[i] = pool.Rent(4);
            }

            for (var i = 0; i < second.Length; i++)
            {
                Assert.AreSame(first[i], second[i], $"Index {i} was not reused");
            }
        }

        [Test]
        public void Rent_OversizedLength_ReturnsExactLength()
        {
            var pool = new SizeSpecificArrayPool<object>(maxLength: 4);

            var array = pool.Rent(8);

            Assert.AreEqual(8, array.Length);
        }

        [Test]
        public void Return_ClearsElementReferences()
        {
            var pool = new SizeSpecificArrayPool<object>(maxLength: 16);

            var array = pool.Rent(2);
            array[0] = new object();
            array[1] = new object();

            pool.Return(array);

            var reused = pool.Rent(2);

            Assert.IsNull(reused[0]);
            Assert.IsNull(reused[1]);
        }
    }
}
