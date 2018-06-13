namespace NeoSharp.Core.Models
{
    public class RegisterTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RegisterTransaction() : base(TransactionType.RegisterTransaction) { }
    }
}