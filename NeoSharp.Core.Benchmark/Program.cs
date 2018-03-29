using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System;

namespace NeoSharp.Network.Benchmark
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
