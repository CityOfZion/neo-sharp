using System;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace NeoSharp.Core.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Summary summary = BenchmarkRunner.Run<BM_PeerFactory>();
            Console.ReadLine();
        }
    }
}
