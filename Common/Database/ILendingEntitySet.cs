using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface ILendingEntitySet : IEntitySet<Lending>
    {
        IList<Lending> GetLendingsOf(int clientId);

        IList<LentBook> GetLentBooksOf(int lendingId);

        void ReturnBooks(Dictionary<int, bool> bookIds, int lendingId);
    }
}
