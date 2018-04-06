using System;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace NeoSharp.Core.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Summary summary = BenchmarkRunner.Run<BMPeerFactory>();
            Console.ReadLine();
        }
    }
}
