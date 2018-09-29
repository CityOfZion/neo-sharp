using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Genesis
{
    public class GenesisBuilder : IGenesisBuilder
    {
        #region Private Fields 
        private readonly IGenesisAssetsBuilder _genesisAssetsBuilder;
        private readonly IBlockSigner _blockSigner;
        #endregion

        #region Constructor
        public GenesisBuilder(IGenesisAssetsBuilder genesisAssetsBuilder, IBlockSigner blockSigner)
        {
            this._genesisAssetsBuilder = genesisAssetsBuilder;
            this._blockSigner = blockSigner;

            BinarySerializer.RegisterTypes(typeof(Transaction).Assembly, typeof(BlockHeader).Assembly);
        }
        #endregion

        #region IGenesisBuilder implementation

        public Block Build()
        {
            var governingToken = this._genesisAssetsBuilder.BuildGoverningTokenRegisterTransaction();
            var utilityToken = this._genesisAssetsBuilder.BuildUtilityTokenRegisterTransaction();

            var genesisMinerTransaction = this._genesisAssetsBuilder.BuildGenesisMinerTransaction();
            var genesisIssueTransaction = this._genesisAssetsBuilder.BuildGenesisIssueTransaction();

            var genesisWitness = this._genesisAssetsBuilder.BuildGenesisWitness();
            var genesisTimestamp = new DateTime(2016, 7, 15, 15, 8, 21, DateTimeKind.Utc).ToTimestamp();
            ulong genesisConsensusData = 2083236893; //向比特币致敬

            var nextConsensusAddress = this._genesisAssetsBuilder.BuildGenesisNextConsensusAddress();

            var genesisBlock = new Block
            {
                PreviousBlockHash = UInt256.Zero,
                Timestamp = genesisTimestamp,
                Index = 0,
                ConsensusData = genesisConsensusData,
                NextConsensus = nextConsensusAddress,
                Witness = genesisWitness,
                Transactions = new Transaction[]
                {
                    //First transaction is always a miner transaction
                    genesisMinerTransaction,
                    //Creates NEO
                    governingToken,
                    //Creates GAS
                    utilityToken,
                    //Send all NEO to seed contract
                    genesisIssueTransaction
                }
            };

            this._blockSigner.Sign(genesisBlock);
            return genesisBlock;
        }
        #endregion
    }
}