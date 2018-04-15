using System;
using BenchmarkDotNet.Running;

namespace NeoSharp.Core.Benchmark
{
    public static class Program
    {
        private static void Main()
        {
            // ReSharper disable once UnusedVariable
            var summary = BenchmarkRunner.Run<BmPeerFactory>();
            Console.ReadLine();
        }
    }
}
