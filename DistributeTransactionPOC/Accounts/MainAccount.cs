using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeTransactionPOC.Accounts
{
    public class MainAccount
    {
        protected long _balance = 0;

        public MainAccount(long initBalance)
        {
            this._balance = initBalance;
        }


        public virtual long GetBalance()
        {
            return this._balance;
        }

        public virtual long Transfer(long value)
        {
            //
            // QUIZ: 確保並行的交易是正確的
            //

            if (this._balance + value < 0) throw new BalanceNotEnoughException();
            return (this._balance += value);

            // throw new NotImplementedException();
        }


        public static bool Transfer(MainAccount from, MainAccount to, long transferAmount)
        {
            //
            // QUIZ: 確保轉帳的過程全部成功，或是全部取消。不能有一方扣款，另一方卻沒取得款項的問題。
            //       即使在不可靠的環境下，仍應該要能順利完成交易。
            //

            try
            {
                from.Transfer(transferAmount);
                to.Transfer(0 - transferAmount);
            }
            catch { }
            return true;

            // throw new NotImplementedException();
        }
    }

}
