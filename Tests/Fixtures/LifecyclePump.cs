using System;
using System.Reflection;
using EgorLin.DIsolated.LifeCycle.Runners;

namespace EgorLin.DIsolated.Tests.Fixtures
{
    public static class LifecyclePump
    {
        private static readonly Action Update = (Action)Delegate.CreateDelegate(
            typeof(Action),
            typeof(RunnerLifeCycleDiContainer).GetMethod("Update", BindingFlags.Static | BindingFlags.NonPublic));

        public static void Pump() => Update();
    }
}
