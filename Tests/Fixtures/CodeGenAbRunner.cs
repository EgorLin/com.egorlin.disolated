using System;
using System.Collections.Generic;
using System.Diagnostics;
using EgorLin.DIsolated.CodeGen;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Data.Types;
using EgorLin.DIsolated.Core.Extensions;

namespace EgorLin.DIsolated.Tests.Fixtures
{
    public static class CodeGenAbRunner
    {
        private static object _sink;

        public static IReadOnlyList<string> ResolveLines()
        {
            return new List<string>
            {
                Run("Transient_Arity0", Arity0, 200000),
                Run("Transient_Arity4", Arity4, 100000),
                Run("Transient_Arity8", Arity8, 100000),
                Run("Transient_Chain9", Chain9, 50000),
                Run("Singleton", Singleton, 200000),
                Run("Enumerable_16_Closed", Enumerable16Closed, 5000),
                Run("Enumerable_16_Dynamic", Enumerable16Dynamic, 5000)
            };
        }

        public static IReadOnlyList<string> MemoryLines()
        {
            var lines = new List<string>();

            foreach (var bindingCount in new[] { 1, 16 })
            {
                var generated = Footprint(bindingCount, true);
                var reflection = Footprint(bindingCount, false);
                lines.Add($"[AB-Memory] container {bindingCount} binding(s) | generated={generated} B | reflection={reflection} B");
            }

            return lines;
        }

        private static string Run(string name, Func<(DiContainer Container, Action Resolve)> scenario, int iterations)
        {
            var (container, resolve) = scenario();

            Warm(resolve, Math.Max(1, iterations / 10));

            var generated = double.MaxValue;
            var reflection = double.MaxValue;

            for (var trial = 0; trial < 7; trial++)
            {
                if ((trial & 1) == 0)
                {
                    generated = Math.Min(generated, Time(resolve, iterations, true));
                    reflection = Math.Min(reflection, Time(resolve, iterations, false));
                }
                else
                {
                    reflection = Math.Min(reflection, Time(resolve, iterations, false));
                    generated = Math.Min(generated, Time(resolve, iterations, true));
                }
            }

            GeneratedConstructorDiscovery.Enabled = true;
            _sink = container;

            return $"[AB] {name} | generated={generated:F1} ns/op | reflection={reflection:F1} ns/op | speedup={reflection / generated:F2}x";
        }

        private static void Warm(Action resolve, int iterations)
        {
            GeneratedConstructorDiscovery.Enabled = true;
            for (var i = 0; i < iterations; i++)
            {
                resolve();
            }

            GeneratedConstructorDiscovery.Enabled = false;
            for (var i = 0; i < iterations; i++)
            {
                resolve();
            }

            GeneratedConstructorDiscovery.Enabled = true;
        }

        private static double Time(Action resolve, int iterations, bool generated)
        {
            GeneratedConstructorDiscovery.Enabled = generated;

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                resolve();
            }

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds * 1000000.0 / iterations;
        }

        private static long Footprint(int bindingCount, bool generated)
        {
            const int containerCount = 2000;
            const int trials = 3;

            GeneratedConstructorDiscovery.Enabled = generated;
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

            GeneratedConstructorDiscovery.Enabled = true;
            return best;
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

        private static DiContainer BuildGraph()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            container.BindSingleton<B, IB>();
            container.BindSingleton<C, IC>();
            container.BindSingleton<D, ID>();
            return container;
        }

        private static (DiContainer, Action) Arity0()
        {
            var container = new DiContainer();
            container.BindTransient<Service0>();
            return (container, () => _sink = container.Resolve<Service0>());
        }

        private static (DiContainer, Action) Arity4()
        {
            var container = BuildGraph();
            container.BindTransient<Service4>();
            return (container, () => _sink = container.Resolve<Service4>());
        }

        private static (DiContainer, Action) Arity8()
        {
            var container = BuildGraph();
            container.BindTransient<Service8>();
            return (container, () => _sink = container.Resolve<Service8>());
        }

        private static (DiContainer, Action) Chain9()
        {
            var container = new DiContainer();
            container.BindTransient<Chain1>();
            container.BindTransient<Chain2>();
            container.BindTransient<Chain3>();
            container.BindTransient<Chain4>();
            container.BindTransient<Chain5>();
            container.BindTransient<Chain6>();
            container.BindTransient<Chain7>();
            container.BindTransient<Chain8>();
            container.BindTransient<Chain9>();
            return (container, () => _sink = container.Resolve<Chain9>());
        }

        private static (DiContainer, Action) Singleton()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            return (container, () => _sink = container.Resolve<IA>());
        }

        private static (DiContainer, Action) Enumerable16Closed()
        {
            var container = new DiContainer();
            container.BindTransient<GenericItem<M0>, IItem>();
            container.BindTransient<GenericItem<M1>, IItem>();
            container.BindTransient<GenericItem<M2>, IItem>();
            container.BindTransient<GenericItem<M3>, IItem>();
            container.BindTransient<GenericItem<M4>, IItem>();
            container.BindTransient<GenericItem<M5>, IItem>();
            container.BindTransient<GenericItem<M6>, IItem>();
            container.BindTransient<GenericItem<M7>, IItem>();
            container.BindTransient<GenericItem<M8>, IItem>();
            container.BindTransient<GenericItem<M9>, IItem>();
            container.BindTransient<GenericItem<M10>, IItem>();
            container.BindTransient<GenericItem<M11>, IItem>();
            container.BindTransient<GenericItem<M12>, IItem>();
            container.BindTransient<GenericItem<M13>, IItem>();
            container.BindTransient<GenericItem<M14>, IItem>();
            container.BindTransient<GenericItem<M15>, IItem>();
            return (container, () => _sink = container.Resolve<IEnumerable<IItem>>());
        }

        private static (DiContainer, Action) Enumerable16Dynamic()
        {
            var container = new DiContainer();
            for (var i = 0; i < Markers.All.Length; i++)
            {
                container.Bind(TypeBinding.Transient, typeof(GenericItem<>).MakeGenericType(Markers.All[i]), typeof(IItem));
            }

            return (container, () => _sink = container.Resolve<IEnumerable<IItem>>());
        }
    }
}
