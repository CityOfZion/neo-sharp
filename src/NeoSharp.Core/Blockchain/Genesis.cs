using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain
{
    public class Genesis
    {
#pragma warning disable CS0612 // Type or member is obsolete

        /// <summary>
        /// Static constructor
        /// </summary>
        static Genesis()
        {
            BinarySerializer.RegisterTypes(typeof(PublishTransaction).Assembly, typeof(BlockHeader).Assembly);

            var governingToken = GenesisAssets.GoverningTokenRegisterTransaction;
            var utilityToken = GenesisAssets.UtilityTokenRegisterTransaction;

            var genesisMinerTransaction = GenesisAssets.GenesisMinerTransaction();
            var genesisIssueTransaction = GenesisAssets.GenesisIssueTransaction();

            var genesisWitness = GenesisAssets.GenesisWitness();
            var genesisTimestamp = (new DateTime(2016, 7, 15, 15, 8, 21, DateTimeKind.Utc)).ToTimestamp();
            ulong genesisConsensusData = 2083236893; //向比特币致敬

            var nextConsensusAddress = GenesisAssets.GetGenesisNextConsensusAddress();

            GenesisBlock = new Block
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

            // Compute hash
            GenesisBlock.UpdateHash();
        }

        /// <summary>
        /// Genesis
        /// </summary>
        public static readonly Block GenesisBlock;
    }
}