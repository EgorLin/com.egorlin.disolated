using System;
using EgorLin.DIsolated.LifeCycle;

namespace EgorLin.DIsolated.Tests.Fixtures
{
    public interface IA { }
    public interface IB { }
    public interface IC { }
    public interface ID { }
    public interface IItem { }
    public interface INotRegistered { }
    public interface IFoo { }
    public interface IBar { }

    public sealed class A : IA
    {
        public A() { }
    }

    public sealed class AAlt : IA
    {
        public AAlt() { }
    }

    public sealed class B : IB
    {
        public IA A { get; }

        public B(IA a) => A = a;
    }

    public sealed class C : IC
    {
        public IA A { get; }
        public IB B { get; }

        public C(IA a, IB b)
        {
            A = a;
            B = b;
        }
    }

    public sealed class D : ID
    {
        public IB B { get; }
        public IC C { get; }

        public D(IB b, IC c)
        {
            B = b;
            C = c;
        }
    }

    public sealed class MultiContract : IA, IB
    {
        public MultiContract() { }
    }

    public sealed class Service0
    {
        public Service0() { }
    }

    public sealed class Service4
    {
        public IA A { get; }
        public IB B { get; }
        public IC C { get; }
        public ID D { get; }

        public Service4(IA a, IB b, IC c, ID d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }
    }

    public sealed class Service8
    {
        public Service8(IA a, IB b, IC c, ID d, IA e, IB f, IC g, ID h)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
            G = g;
            H = h;
        }

        public IA A { get; }
        public IB B { get; }
        public IC C { get; }
        public ID D { get; }
        public IA E { get; }
        public IB F { get; }
        public IC G { get; }
        public ID H { get; }
    }

    public sealed class Chain1 { public Chain1() { } }

    public sealed class Chain2 { public Chain2(Chain1 value) => Value = value; public Chain1 Value { get; } }

    public sealed class Chain3 { public Chain3(Chain2 value) => Value = value; public Chain2 Value { get; } }

    public sealed class Chain4 { public Chain4(Chain3 value) => Value = value; public Chain3 Value { get; } }

    public sealed class Chain5 { public Chain5(Chain4 value) => Value = value; public Chain4 Value { get; } }

    public sealed class Chain6 { public Chain6(Chain5 value) => Value = value; public Chain5 Value { get; } }

    public sealed class Chain7 { public Chain7(Chain6 value) => Value = value; public Chain6 Value { get; } }

    public sealed class Chain8 { public Chain8(Chain7 value) => Value = value; public Chain7 Value { get; } }

    public sealed class Chain9 { public Chain9(Chain8 value) => Value = value; public Chain8 Value { get; } }

    public sealed class Chain10 { public Chain10(Chain9 value) => Value = value; public Chain9 Value { get; } }

    public sealed class GenericItem<T> : IItem
    {
        public GenericItem() { }
    }

    public sealed class OptionalDependency
    {
        public IA A { get; }
        public int Count { get; }

        public OptionalDependency(IA a, int count = 5)
        {
            A = a;
            Count = count;
        }
    }

    public sealed class NeedsNotRegistered
    {
        public NeedsNotRegistered(INotRegistered value) { }
    }

    public sealed class TwoCtor
    {
        public bool UsedDependency { get; }
        public IA A { get; }

        public TwoCtor() => UsedDependency = false;

        public TwoCtor(IA a)
        {
            UsedDependency = true;
            A = a;
        }
    }

    public struct PlainStruct
    {
        public int Value;
    }

    public sealed class Foo : IFoo
    {
        public Foo() { }
    }

    public sealed class Bar : IBar
    {
        public Bar() { }
    }

    public sealed class TestDependency
    {
    }

    public sealed class TestBootstrap : IBootstrap
    {
        public int BootCount { get; private set; }

        public void Boot() => BootCount++;
    }

    public sealed class TestTickable : ITickable
    {
        public int TickCount { get; private set; }

        public void Tick() => TickCount++;
    }

    public sealed class RecordingDisposable : IDisposable
    {
        private readonly System.Collections.Generic.List<int> _log;
        private readonly int _id;

        public RecordingDisposable(System.Collections.Generic.List<int> log, int id)
        {
            _log = log;
            _id = id;
        }

        public void Dispose() => _log.Add(_id);
    }
}
