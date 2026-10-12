using EgorLin.DIsolated.Pools;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace EgorLin.DIsolated.Tests.PlayMode
{
    public class PoolBenchmarks
    {
        [Test, Performance]
        public void RentReturn_16Concurrent_Arity4()
        {
            var pool = new SizeSpecificArrayPool<object>(maxLength: 16);
            var buffer = new object[16][];

            Measure.Method(() =>
                {
                    for (var i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = pool.Rent(4);
                    }

                    for (var i = 0; i < buffer.Length; i++)
                    {
                        pool.Return(buffer[i]);
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }
    }
}
