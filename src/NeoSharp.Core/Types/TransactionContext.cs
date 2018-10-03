using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Types
{
    public class TransactionContext : ITransactionContext
    {
        public Fixed8 DefaultSystemFee => Fixed8.Zero; // Settings.Default.SystemFee.TryGetValue(Type, out Fixed8 fee) ? fee : Fixed8.Zero;
        public UInt256 UtilityTokenHash => UInt256.Zero;
        public UInt256 GoverningTokenHash => UInt256.Zero;

        // ---------------------------

        private IReadOnlyDictionary<CoinReference, TransactionOutput> _references;

        private Fixed8 _systemFee = -Fixed8.Satoshi, _network_fee = -Fixed8.Satoshi;

        /// <summary>
        /// System Fee
        /// </summary>
        public Fixed8 GetSystemFee(Transaction tx)
        {
            if (_systemFee == -Fixed8.Satoshi)
            {
                _systemFee = DefaultSystemFee;

                switch (tx.Type)
                {
                    case TransactionType.InvocationTransaction:
                        {
                            InvocationTransaction itx = (InvocationTransaction)tx;
                            _systemFee = itx.Gas;

                            break;
                        }
                    case TransactionType.IssueTransaction:
                        {
                            if (tx.Version >= 1)
                            {
                                _systemFee = Fixed8.Zero;
                                return _systemFee;
                            }

                            if (tx.Outputs.All(p => p.AssetId == GoverningTokenHash || p.AssetId == UtilityTokenHash))
                                _systemFee = Fixed8.Zero;

                            break;
                        }
                    case TransactionType.RegisterTransaction:
                        {
                            var rtx = (RegisterTransaction) tx;

                            if (rtx.AssetType == AssetType.GoverningToken || rtx.AssetType == AssetType.UtilityToken)
                                _systemFee = Fixed8.Zero;

                            break;
                        }
                    case TransactionType.StateTransaction:
                        {
                            StateTransaction stx = (StateTransaction)tx;
                            _systemFee = new Fixed8(stx.Descriptors.Sum(p => p.SystemFee.Value));

                            break;
                        }
                }
            }
            return _systemFee;
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
    }
}