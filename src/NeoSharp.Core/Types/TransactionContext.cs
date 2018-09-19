using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Types
{
    public class TransactionContext
    {
        // TODO #362: How to inject this

        public static readonly Fixed8 DefaultSystemFee = Fixed8.Zero;// Settings.Default.SystemFee.TryGetValue(Type, out Fixed8 fee) ? fee : Fixed8.Zero;
        public static readonly UInt256 UtilityTokenHash = UInt256.Zero, GoverningTokenHash = UInt256.Zero;

        // ---------------------------

        private IReadOnlyDictionary<CoinReference, TransactionOutput> _references;
        public readonly IBlockchain Blockchain = null;

        private readonly Transaction _tx;
        private Fixed8 _systemFee = -Fixed8.Satoshi, _network_fee = -Fixed8.Satoshi;

        /// <summary>
        /// System Fee
        /// </summary>
        public Fixed8 SystemFee
        {
            get
            {
                if (_systemFee == -Fixed8.Satoshi)
                {
                    _systemFee = DefaultSystemFee;

                    switch (_tx.Type)
                    {
                        case TransactionType.InvocationTransaction:
                            {
                                InvocationTransaction itx = (InvocationTransaction)_tx;
                                _systemFee = itx.Gas;

                                break;
                            }
                        case TransactionType.IssueTransaction:
                            {
                                if (_tx.Version >= 1)
                                {
                                    _systemFee = Fixed8.Zero;
                                    return _systemFee;
                                }

                                if (_tx.Outputs.All(p => p.AssetId == GoverningTokenHash || p.AssetId == UtilityTokenHash))
                                    _systemFee = Fixed8.Zero;

                                break;
                            }
                        case TransactionType.RegisterTransaction:
                            {
                                var rtx = (RegisterTransaction) _tx;

                                if (rtx.AssetType == AssetType.GoverningToken || rtx.AssetType == AssetType.UtilityToken)
                                    _systemFee = Fixed8.Zero;

                                break;
                            }
                        case TransactionType.StateTransaction:
                            {
                                StateTransaction stx = (StateTransaction)_tx;
                                _systemFee = new Fixed8(stx.Descriptors.Sum(p => p.SystemFee.Value));

                                break;
                            }
                    }
                }
                return _systemFee;
            }
        }

        // TODO #361 [AboimPinto]: This logic need to be done in different way because we cannot await a method in the 'get' body.
        /// <summary>
        /// Network fee
        /// </summary>
        //public virtual Fixed8 NetworkFee
        //{
        //    get
        //    {
        //        if (_network_fee == -Fixed8.Satoshi)
        //        {
        //            switch (Tx.Type)
        //            {
        //                case TransactionType.MinerTransaction:
        //                    {
        //                        _network_fee = Fixed8.Zero;
        //                        break;
        //                    }
        //                default:
        //                    {
        //                        var input = new Fixed8(this.References.Values.Where(p => p.AssetId.Equals(UtilityTokenHash)).Sum(p => p.Value.Value));

        //                        var output = new Fixed8(Tx.Outputs.Where(p => p.AssetId.Equals(UtilityTokenHash)).Sum(p => p.Value.Value));

        //                        _network_fee = input - output - SystemFee;

        //                        break;
        //                    }
        //            }
        //        }
        //        return _network_fee;
        //    }
        //}

        /// <summary>
        /// Each transaction inputs the quoted transaction output
        /// </summary>
        //public IReadOnlyDictionary<CoinReference, TransactionOutput> References
        //{
        //    get
        //    {
        //        if (_references == null)
        //        {
        //            var dictionary = new Dictionary<CoinReference, TransactionOutput>();
        //            foreach (var group in Tx.Inputs.GroupBy(p => p.PrevHash))
        //            {
        //                var tx = this._blockchain?.GetTransaction(group.Key);
        //                if (tx == null) return null;

        //                foreach (var reference in group.Select(p => new
        //                {
        //                    Input = p,
        //                    Output = tx.Outputs[p.PrevIndex]
        //                }))
        //                {
        //                    dictionary.Add(reference.Input, reference.Output);
        //                }
        //            }

        //            _references = dictionary;
        //        }
        //        return _references;
        //    }
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tx">Transaction</param>
        /// <param name="blockchain">Blockchain</param>
        public TransactionContext(Transaction tx, IBlockchain blockchain)
        {
            _tx = tx;
            Blockchain = blockchain;
        }
    }
}