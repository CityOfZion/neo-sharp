using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain.Processors
{
    [TestClass]
    public class UtEnrollmentTransactionPersister : TestBase
    {
        [TestMethod]
        public async Task Persist_SetValidatorRegisteredToTrue()
        {
            var pubKey = new byte[33];
            pubKey[0] = 0x02;
            var input = new EnrollmentTransaction
            {
                PublicKey = new ECPoint(pubKey)
            };
            var validator = new Validator
            {
                Registered = false
            };
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock.Setup(m => m.GetValidator(input.PublicKey)).ReturnsAsync(validator);
            var testee = AutoMockContainer.Create<EnrollmentTransactionPersister>();

            await testee.Persist(input);

            repositoryMock.Verify(m => m.AddValidator(It.Is<Validator>(v => v == validator && v.Registered)));
        }
    }
}