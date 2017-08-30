using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributeTransactionPOC.Accounts
{

    public class NetworkMainAccount : MainAccount
    {
        public NetworkMainAccount(long initBalance) : base(initBalance)
        {

        }


        private static Random _rnd = new Random();
        private const int error_rate = 100;

        public override long GetBalance()
        {
            if (_rnd.Next(error_rate) == 0) throw new NetworkException();
            return base.GetBalance();
        }

        public override long Transfer(long value)
        {
            if (_rnd.Next(error_rate * 2) == 0) throw new NetworkException();
            base.Transfer(value);
            if (_rnd.Next(error_rate * 2) == 0) throw new NetworkException();
            return this._balance;
        }
    }
}
