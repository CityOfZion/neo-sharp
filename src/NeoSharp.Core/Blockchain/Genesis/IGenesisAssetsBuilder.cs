using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Genesis
{
    public interface IGenesisAssetsBuilder
    {
        RegisterTransaction BuildGoverningTokenRegisterTransaction();

        RegisterTransaction BuildUtilityTokenRegisterTransaction();

        MinerTransaction BuildGenesisMinerTransaction();

        IssueTransaction BuildGenesisIssueTransaction();

        Witness BuildGenesisWitness();

        UInt160 BuildGenesisNextConsensusAddress();
    }
}