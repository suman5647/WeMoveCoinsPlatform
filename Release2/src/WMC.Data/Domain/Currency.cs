namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using WMC.Data.Enums;

    [Table("Currency")]
    public partial class Currency
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Currency()
        {
            Countries = new List<Country>();
            CryptoCurrencyOrders = new List<Order>();
            CurrencyOrders = new List<Order>();
            Transactions = new List<Transaction>();
        }

        public long Id { get; set; }

        public long? CurrencyTypeId { get; set; }

        [Required]
        [StringLength(3)]
        public string Code { get; set; }

        [StringLength(40)]
        public string Text { get; set; }

        [StringLength(10)]
        public string YourPayCurrencyCode { get; set; }

        [StringLength(10)]
        public string PayLikeCurrencyCode { get; set; }

        public string PayLikeDetails { get; set; }

        public decimal? FXMarkUp { get; set; }

        public string BitgoSettings { get; set; }

        public bool IsActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Country> Countries { get; set; }

        public virtual CurrencyType CurrencyType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Order> CryptoCurrencyOrders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Order> CurrencyOrders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Transaction> Transactions { get; set; }

        public CurrencyAcceptance PaymentTypeAcceptance { get; set; }

        public int MinorUnits { get; set; }
    }
}
