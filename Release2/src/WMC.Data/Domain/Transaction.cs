namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Transaction")]
    public partial class Transaction
    {
        public long Id { get; set; }

        public long? OrderId { get; set; }

        public long MethodId { get; set; }

        public long Type { get; set; }

        public string ExtRef { get; set; }

        public decimal? Amount { get; set; }

        public long Currency { get; set; }

        public string Info { get; set; }

        public DateTime? Completed { get; set; }

        public long? FromAccount { get; set; }

        public long? ToAccount { get; set; }

        public DateTime? Reconsiled { get; set; }

        public DateTime? Exported { get; set; }

        public virtual Account FromAccountRef { get; set; }

        public virtual Account ToAccountRef { get; set; }

        public virtual Currency CurrencyRef { get; set; }

        public virtual Order Order { get; set; }

        public virtual TransactionMethod TransactionMethod { get; set; }

        public virtual TransactionType TransactionType { get; set; }

        public string BatchNumber { get; set; }
    }
}
