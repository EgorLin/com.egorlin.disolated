using EgorLin.DIsolated.Tests.Fixtures;
using NUnit.Framework;

namespace EgorLin.DIsolated.Benchmarks
{
    [Explicit("A/B codegen benchmark; run with --filter \"EgorLin.DIsolated.Benchmarks\".")]
    public class CodeGenAbBenchmarks
    {
        [Test]
        public void Resolve_Generated_vs_Reflection()
        {
            foreach (var line in CodeGenAbRunner.ResolveLines())
            {
                TestContext.WriteLine(line);
            }
        }

        [Test]
        public void Memory_ContainerFootprint_Generated_vs_Reflection()
        {
            foreach (var line in CodeGenAbRunner.MemoryLines())
            {
                TestContext.WriteLine(line);
            }
        }
    }
}
