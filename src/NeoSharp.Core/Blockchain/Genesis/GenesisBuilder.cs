using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Genesis
{
    public class GenesisBuilder : IGenesisBuilder
    {
        #region Private Fields 

        private Block _genesisBlock;
        private readonly IGenesisAssetsBuilder _genesisAssetsBuilder;
        private readonly ISigner<Block> _blockSigner;

        #endregion

        #region Constructor

        public GenesisBuilder(IGenesisAssetsBuilder genesisAssetsBuilder, ISigner<Block> blockSigner)
        {
            _genesisAssetsBuilder = genesisAssetsBuilder;
            _blockSigner = blockSigner;

            BinarySerializer.RegisterTypes(typeof(Transaction).Assembly, typeof(BlockHeader).Assembly);
        }

        #endregion

        #region IGenesisBuilder implementation

        public Block Build()
        {
            if (_genesisBlock != null) return _genesisBlock;

            var governingToken = _genesisAssetsBuilder.BuildGoverningTokenRegisterTransaction();
            var utilityToken = _genesisAssetsBuilder.BuildUtilityTokenRegisterTransaction();

            var genesisMinerTransaction = _genesisAssetsBuilder.BuildGenesisMinerTransaction();
            var genesisIssueTransaction = _genesisAssetsBuilder.BuildGenesisIssueTransaction();

            var genesisWitness = _genesisAssetsBuilder.BuildGenesisWitness();
            var genesisTimestamp = new DateTime(2016, 7, 15, 15, 8, 21, DateTimeKind.Utc).ToTimestamp();
            ulong genesisConsensusData = 2083236893;

            var nextConsensusAddress = _genesisAssetsBuilder.BuildGenesisNextConsensusAddress();

            _genesisBlock = new Block
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

            _blockSigner.Sign(_genesisBlock);
            return _genesisBlock;
        }

        #endregion
    }
}