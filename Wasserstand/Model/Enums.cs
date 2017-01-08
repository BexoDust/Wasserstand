namespace Wasserstand.Model
{
    public class Enums
    {
        public enum Currency
        {
            Euro,
            Dollar
        }

        public enum TransactionType
        {
            Lastschrift,
            Dauerauftrag,
            Auszahlung,
            Gutschrift,
            Zinsen
        }

        public enum BankType
        {
            Commerzbank,
            Dkb
        }

        public enum TagType
        {
            Description,
            Amount,
            Type
        }

        public static Currency CurrencyParser(string currencyString)
        {
            Currency result = Currency.Euro;

            switch (currencyString)
            {
                case "EUR":
                    result = Currency.Euro;
                    break;
                case "DOL":
                    result = Currency.Dollar;
                    break;
            }

            return result;
        }

        public static string CurrencyToSymbol(Currency currency)
        {
            string result = "€";

            switch (currency)
            {
                case Currency.Euro:
                    result = "€";
                    break;
                case Currency.Dollar:
                    result = "$";
                    break;
            }

            return result;
        }

        public static TransactionType TransactionTypeParser(string typeString)
        {
            TransactionType result = TransactionType.Lastschrift;

            switch (typeString)
            {
                case "Lastschrift":
                    result = TransactionType.Lastschrift;
                    break;
                case "Dauerauftrag":
                    result = TransactionType.Dauerauftrag;
                    break;
                case "Gutschrift":
                    result = TransactionType.Gutschrift;
                    break;
                case "Einzahlung/Auszahlung":
                    result = TransactionType.Auszahlung;
                    break;
                case "Zinsen/Entgelte":
                    result = TransactionType.Zinsen;
                    break;
            }

            return result;
        }
    }
}
