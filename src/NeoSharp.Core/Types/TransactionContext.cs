using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Types
{
    public class TransactionContext
    {
        // TODO: How to inject this

        public readonly static Fixed8 DefaultSystemFee = Fixed8.Zero;// Settings.Default.SystemFee.TryGetValue(Type, out Fixed8 fee) ? fee : Fixed8.Zero;
        public readonly static UInt256 UtilityTokenHash = UInt256.Zero, GoverningTokenHash = UInt256.Zero;

        // ---------------------------

        private IReadOnlyDictionary<CoinReference, TransactionOutput> _references;
        public readonly IBlockchain _blockchain = null;

        private readonly Transaction Tx;
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

                    switch (Tx.Type)
                    {
                        case TransactionType.InvocationTransaction:
                            {
                                InvocationTransaction itx = (InvocationTransaction)Tx;
                                _systemFee = itx.Gas;

                                break;
                            }
                        case TransactionType.IssueTransaction:
                            {
                                if (Tx.Version >= 1)
                                {
                                    _systemFee = Fixed8.Zero;
                                    return _systemFee;
                                }

                                if (Tx.Outputs.All(p => p.AssetId == GoverningTokenHash || p.AssetId == UtilityTokenHash))
                                    _systemFee = Fixed8.Zero;

                                break;
                            }
                        case TransactionType.RegisterTransaction:
                            {
                                RegisterTransaction rtx = (RegisterTransaction)Tx;

                                if (rtx.AssetType == AssetType.GoverningToken || rtx.AssetType == AssetType.UtilityToken)
                                    _systemFee = Fixed8.Zero;

                                break;
                            }
                        case TransactionType.StateTransaction:
                            {
                                StateTransaction stx = (StateTransaction)Tx;
                                _systemFee = new Fixed8(stx.Descriptors.Sum(p => p.SystemFee.Value));

                                break;
                            }
                    }
                }
                return _systemFee;
            }
        }

        /// <summary>
        /// Network fee
        /// </summary>
        public virtual Fixed8 NetworkFee
        {
            get
            {
                if (_network_fee == -Fixed8.Satoshi)
                {
                    switch (Tx.Type)
                    {
                        case TransactionType.MinerTransaction:
                            {
                                _network_fee = Fixed8.Zero;
                                break;
                            }
                        default:
                            {
                                Fixed8 input = new Fixed8(References.Values.Where(p => p.AssetId.Equals(UtilityTokenHash)).Sum(p => p.Value.Value));
                                Fixed8 output = new Fixed8(Tx.Outputs.Where(p => p.AssetId.Equals(UtilityTokenHash)).Sum(p => p.Value.Value));
                                _network_fee = input - output - SystemFee;
                                break;
                            }
                    }
                }
                return _network_fee;
            }
        }

        /// <summary>
        /// Each transaction inputs the quoted transaction output
        /// </summary>
        public IReadOnlyDictionary<CoinReference, TransactionOutput> References
        {
            get
            {
                if (_references == null)
                {
                    Dictionary<CoinReference, TransactionOutput> dictionary = new Dictionary<CoinReference, TransactionOutput>();
                    foreach (var group in Tx.Inputs.GroupBy(p => p.PrevHash))
                    {
                        Transaction tx = _blockchain?.GetTransaction(group.Key);
                        if (tx == null) return null;

                        foreach (var reference in group.Select(p => new
                        {
                            Input = p,
                            Output = tx.Outputs[p.PrevIndex]
                        }))
                        {
                            dictionary.Add(reference.Input, reference.Output);
                        }
                    }

                    _references = dictionary;
                }
                return _references;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tx">Transaction</param>
        /// <param name="blockchain">Blockchain</param>
        public TransactionContext(Transaction tx, IBlockchain blockchain)
        {
            Tx = tx;
            _blockchain = blockchain;
        }
    }
}