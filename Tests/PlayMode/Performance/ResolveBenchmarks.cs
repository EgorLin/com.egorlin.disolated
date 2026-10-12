using System;
using System.Collections.Generic;
using System.Diagnostics;
using EgorLin.DIsolated.Core;
using EgorLin.DIsolated.Core.Data.Types;
using EgorLin.DIsolated.Core.Extensions;
using EgorLin.DIsolated.LifeCycle;
using EgorLin.DIsolated.LifeCycle.Runners;
using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace EgorLin.DIsolated.Tests.PlayMode
{
    public class ResolveBenchmarks
    {
        private static object _sink;

        [SetUp]
        public void SetUp()
        {
            RunnerLifeCycleDiContainer.ClearAll();
        }

        [TearDown]
        public void TearDown()
        {
            RunnerLifeCycleDiContainer.ClearAll();
        }

        [Test, Performance]
        public void Singleton_Warm()
        {
            var container = BuildDependencyGraph();
            _sink = container.Resolve<IA>();

            Measure.Method(() => _sink = container.Resolve<IA>())
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Transient_Warm_Arity0()
        {
            var container = new DiContainer();
            container.BindTransient<Service0>();
            _sink = container.Resolve<Service0>();

            Measure.Method(() => _sink = container.Resolve<Service0>())
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Transient_Warm_Arity4()
        {
            var container = BuildDependencyGraph();
            container.BindTransient<Service4>();
            _sink = container.Resolve<Service4>();

            Measure.Method(() => _sink = container.Resolve<Service4>())
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Transient_Warm_Arity8()
        {
            var container = BuildDependencyGraph();
            container.BindTransient<Service8>();
            _sink = container.Resolve<Service8>();

            Measure.Method(() => _sink = container.Resolve<Service8>())
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Transient_Warm_GraphDepth9()
        {
            var container = BuildChain9();
            _sink = container.Resolve<Chain9>();

            Measure.Method(() => _sink = container.Resolve<Chain9>())
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void ScopeDepth_1()
        {
            RunScopeDepth(1);
        }

        [Test, Performance]
        public void ScopeDepth_5()
        {
            RunScopeDepth(5);
        }

        [Test, Performance]
        public void Enumerable_16_Bindings()
        {
            var container = BuildEnumerable(16);
            _sink = container.Resolve<IEnumerable<IItem>>();

            Measure.Method(() => _sink = container.Resolve<IEnumerable<IItem>>())
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Registration_And_Build()
        {
            Measure.Method(() =>
                {
                    var container = new DiContainer();
                    container.BindSingleton<A, IA>();
                    container.BindSingleton<B, IB>();
                    for (var i = 0; i < Markers.All.Length; i++)
                    {
                        container.Bind(TypeBinding.Singleton, typeof(GenericItem<>).MakeGenericType(Markers.All[i]));
                    }

                    container.Build();
                })
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(100)
                .GC()
                .CleanUp(RunnerLifeCycleDiContainer.ClearAll)
                .Run();
        }

        [Test, Performance]
        public void Lifecycle_Tick_128Tickables()
        {
            var container = new DiContainer();
            for (var i = 0; i < 128; i++)
            {
                container.BindInstance<ITickable>(new TestTickable());
            }

            container.Build();
            LifecyclePump.Pump();

            Measure.Method(LifecyclePump.Pump)
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(100)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Cold_FirstResolve_16DistinctTypes()
        {
            var sample = new SampleGroup("ColdResolve", SampleUnit.Millisecond);

            foreach (var marker in Markers.All)
            {
                var closedType = typeof(GenericItem<>).MakeGenericType(marker);
                var container = new DiContainer();
                container.Bind(TypeBinding.Transient, closedType);

                var stopwatch = Stopwatch.StartNew();
                _sink = container.Resolve(closedType);
                stopwatch.Stop();

                Measure.Custom(sample, stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        private static void RunScopeDepth(int depth)
        {
            var root = new DiContainer();
            root.BindSingleton<A, IA>();

            var current = root;
            for (var i = 0; i < depth; i++)
            {
                current = new DiContainer(current);
            }

            _sink = current.Resolve<IA>();

            Measure.Method(() => _sink = current.Resolve<IA>())
                .WarmupCount(5)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        private static DiContainer BuildDependencyGraph()
        {
            var container = new DiContainer();
            container.BindSingleton<A, IA>();
            container.BindSingleton<B, IB>();
            container.BindSingleton<C, IC>();
            container.BindSingleton<D, ID>();
            return container;
        }

        private static DiContainer BuildEnumerable(int count)
        {
            var container = new DiContainer();
            for (var i = 0; i < count; i++)
            {
                container.Bind(TypeBinding.Transient, typeof(GenericItem<>).MakeGenericType(Markers.All[i]), typeof(IItem));
            }

            return container;
        }

        private static DiContainer BuildChain9()
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
            return container;
        }
    }
}
