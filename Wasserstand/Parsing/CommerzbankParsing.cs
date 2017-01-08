using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasserstand.Model;
using Windows.Storage;
using static Wasserstand.Model.Enums;

namespace Wasserstand.Parsing
{
    public class CommerzbankParsing : IParsing
    {
        public BankType GetBankType()
        {
            return BankType.Commerzbank;
        }

        public async Task<IList<TransactionEntry>> ReadTransactions(StorageFile file)
        {
            //string[] allLines = File.ReadAllLines(path);
            IList<string> allLines = await FileIO.ReadLinesAsync(file);
            List<string> newList = new List<string>(allLines);
            newList.RemoveAt(0);

            //string dateString = newList[0].Split('M')

            var query = from line in newList
                        let data = line.Split(';')
                        select new TransactionEntry(DateTime.Parse(data[0]), Double.Parse(data[4]), data[3], "Andere", 
                        Enums.CurrencyParser(data[5]), Enums.TransactionTypeParser(data[2]));

            return query.ToList();
        }
    }
}
