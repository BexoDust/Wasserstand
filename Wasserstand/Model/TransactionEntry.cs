using System;
using WpfHelper;
using static Wasserstand.Model.Enums;

namespace Wasserstand.Model
{
    public class TransactionEntry : ANotifyBase, IComparable<TransactionEntry>, IEquatable<TransactionEntry>
    {

        public DateTime Date
        {
            get { return Get(() => Date); }
            set { Set(value, () => Date); }
        }

        public double Amount
        {
            get { return Get(() => Amount); }
            set { Set(value, () => Amount); }
        }

        public string Description
        {
            get { return Get(() => Description); }
            set { Set(value, () => Description); }
        }

        public string Tag
        {
            get { return Get(() => Tag); }
            set { Set(value, () => Tag); }
        }


        public double Total
        {
            get { return Get(() => Total); }
            set { Set(value, () => Total); }
        }


        public double TagTotal
        {
            get { return Get(() => TagTotal); }
            set { Set(value, () => TagTotal); }
        }

        public Currency Currency
        {
            get { return Get(() => Currency); }
            set
            {
                Set(value, () => Currency);
                OnPropertyChanged("CurrencySymbol");
            }
        }

        public string CurrencySymbol
        {
            get
            {
               return Enums.CurrencyToSymbol(this.Currency);
            }
        }

        public TransactionType Type
        {
            get { return Get(() => Type); }
            set { Set(value, () => Type); }
        }

        public TransactionEntry() : this(DateTime.MinValue, 0.0, String.Empty, "Andere", Currency.Euro, TransactionType.Lastschrift)
        {

        }

        public TransactionEntry(DateTime date, double amount, string description, string tag, Currency currency, TransactionType type)
        {
            this.Date = date;
            this.Amount = amount;
            this.Description = description;
            this.Tag = tag;
            this.Currency = currency;
            this.Type = type;
        }

        public int CompareTo(TransactionEntry other)
        {
            return this.Date.CompareTo(other.Date);
        }

        public bool Equals(TransactionEntry other)
        {
            return this.Date.Equals(other.Date);
        }
    }
}
