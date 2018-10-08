using System;
using System.Text;
using Moq;
using NeoSharp.TestHelpers.AutoMock;

namespace NeoSharp.TestHelpers
{
    public abstract class TestBase
    {
        private readonly MockRepository _mockRepository;
        private readonly Random _rand;

        private const string RandStringAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly int RandStringAlphabetLength = RandStringAlphabet.Length;

        public IAutoMockContainer AutoMockContainer { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        protected TestBase()
        {
            _rand = new Random(Environment.TickCount);
            _mockRepository = new MockRepository(MockBehavior.Loose);
            AutoMockContainer = new UnityAutoMockContainer(_mockRepository);
        }

        /// <summary>
        /// Generate random strings
        /// </summary>
        /// <param name="length">String lenght</param>
        /// <returns>String</returns>
        public string RandomString(int length)
        {
            var result = new byte[length];
            _rand.NextBytes(result);

            for (int i = 0; i < length; ++i)
            {
                result[i] = (byte)RandStringAlphabet[result[i] % RandStringAlphabetLength];
            }

            return Encoding.ASCII.GetString(result);
        }

        /// <summary>
        /// Generate a random integer
        /// </summary>
        /// <returns>A positive integer</returns>
        public int RandomInt()
        {
            return _rand.Next();
        }

        /// <summary>
        /// Generate a random integer with a max value
        /// </summary>
        /// <param name="max"></param>
        /// <returns>A positive integer that is smaller than max</returns>
        public int RandomInt(int max)
        {
            return _rand.Next(max);
        }

        public int RandomInt(int minValue, int maxValue)
        {
            return _rand.Next(minValue, maxValue);
        }

        public byte[] RandomByteArray(int len)
        {
            var output = new byte[len];
            _rand.NextBytes(output);
            return output;
        }

        /// <summary>
        /// Verify All
        /// </summary>
        public void VerifyAll()
        {
            _mockRepository.VerifyAll();
        }
    }
}