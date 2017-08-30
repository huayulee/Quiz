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
            // TODO: 確保並行的交易是正確的
            //

            if (this._balance + value < 0) throw new BalanceNotEnoughException();
            return (this._balance += value);
        }
    }

}
