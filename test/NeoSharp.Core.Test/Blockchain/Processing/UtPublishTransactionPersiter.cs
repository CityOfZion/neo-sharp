using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.SmartContract;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain.Processing
{
    [TestClass]
    public class UtPublishTransactionPersiter : TestBase
    {
        [TestMethod]
        public async Task Persist_AddContract()
        {
            var input = new PublishTransaction
            {
                Script = RandomByteArray(10),
                Description = RandomString(10),
                ReturnType = (ContractParameterType) RandomInt(10),
                ParameterList = new[] {(ContractParameterType) RandomInt(10), (ContractParameterType) RandomInt(10)},
                Name = RandomString(10),
                Version = (byte) RandomInt(10),
                Author = RandomString(10),
                Email = RandomString(10)
            };
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            var testee = AutoMockContainer.Create<PublishTransactionPersister>();

            await testee.Persist(input);
            repositoryMock.Verify(m => m.AddContract(It.Is<Contract>(c =>
                c.Code != null &&
                c.Code.ScriptHash.Equals(input.Script.ToScriptHash()) &&
                c.Code.Script == input.Script &&
                c.Code.ReturnType == input.ReturnType &&
                c.Code.Parameters == input.ParameterList &&
                c.Name == input.Name &&
                c.Version == input.CodeVersion &&
                c.Author == input.Author &&
                c.Email == input.Email &&
                c.Description == input.Description)));
        }
    }
}