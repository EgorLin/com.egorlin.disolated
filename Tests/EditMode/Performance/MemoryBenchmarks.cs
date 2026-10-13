using System;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;

namespace EgorLin.DIsolated.MemoryProbe
{
    [Explicit("Memory probe; run explicitly with --filter \"EgorLin.DIsolated.MemoryProbe\". Prints [Memory] lines to the test output.")]
    public class MemoryBenchmarks
    {
        private static object _sink;

        [Test]
        public void Memory_ContainerFootprint_1Binding()
        {
            Measure(1);
        }

        [Test]
        public void Memory_ContainerFootprint_16Bindings()
        {
            Measure(16);
        }

        private static void Measure(int bindingCount)
        {
            const int containerCount = 2000;
            const int trials = 3;

            BuildContainer(bindingCount);

            var best = long.MaxValue;

            for (var trial = 0; trial < trials; trial++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var before = GC.GetTotalMemory(true);

                var containers = new DiContainer[containerCount];
                for (var i = 0; i < containerCount; i++)
                {
                    containers[i] = BuildContainer(bindingCount);
                }

                var after = GC.GetTotalMemory(true);
                _sink = containers;

                var perContainer = (after - before) / containerCount;
                if (perContainer < best)
                {
                    best = perContainer;
                }
            }

            TestContext.WriteLine($"[Memory] live bytes/container ({bindingCount} binding(s)): {best}");
        }

        private static DiContainer BuildContainer(int bindingCount)
        {
            var container = new DiContainer();

            for (var i = 0; i < bindingCount; i++)
            {
                container.BindTransient(typeof(GenericItem<>).MakeGenericType(Markers.All[i]));
            }

            return container;
        }
    }
}
