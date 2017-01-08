using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using WpfHelper;
using static Wasserstand.Model.Enums;

namespace Wasserstand.Model
{
    public class Tag : ANotifyBase, IComparable<Tag>, IEquatable<Tag>
    {

        
        public string Name
        {
            get { return Get(() => Name); }
            set { Set(value, () => Name); }
        }


        public string Description
        {
            get { return Get(() => Description); }
            set { Set(value, () => Description); }
        }


        public double Amount
        {
            get { return Get(() => Amount); }
            set { Set(value, () => Amount); }
        }


        public TransactionType Transaction
        {
            get { return Get(() => Transaction); }
            set { Set(value, () => Transaction); }
        }


        public TagType Type
        {
            get { return Get(() => Type); }
            set { Set(value, () => Type); }
        }

        [XmlIgnoreAttribute]
        public List<TagType> TypeList
        {
            get
            {
                return Enum.GetValues(typeof(TagType)).Cast<TagType>().ToList();
            }
        }

        [XmlIgnoreAttribute]
        public List<TransactionType> TransactionList
        {
            get
            {
                return Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>().ToList();
            }
        }


        public int Order
        {
            get { return Get(() => Order); }
            set { Set(value, () => Order); }
        }

        public Tag()
        {
            this.Name = String.Empty;
            this.Amount = 0;
            this.Transaction = TransactionType.Auszahlung;
            this.Type = TagType.Description;

        }

        public int CompareTo(Tag other)
        {
            return this.Order.CompareTo(other.Order);
        }

        public bool Equals(Tag other)
        {
            return this.Order.Equals(other.Order);
        }
    }
}
