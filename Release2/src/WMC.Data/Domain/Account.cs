namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Account")]
    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            FromAccountTransactions = new List<Transaction>();
            ToAccountTransactions = new List<Transaction>();
        }

        public long Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Text { get; set; }
        public long Type { get; set; }
        public long Currency { get; set; }
        public string ValueFor { get; set; }
        public int TransactionType { get; set; }
        public int ParticularType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Transaction> FromAccountTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Transaction> ToAccountTransactions { get; set; }
    }
}
