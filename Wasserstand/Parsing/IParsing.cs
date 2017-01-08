using System.Collections.Generic;
using System.Threading.Tasks;
using Wasserstand.Model;
using Windows.Storage;
using static Wasserstand.Model.Enums;

namespace Wasserstand.Parsing
{
    public interface IParsing
    {
        BankType GetBankType();

        Task<IList<TransactionEntry>> ReadTransactions(StorageFile file);
    }
}
