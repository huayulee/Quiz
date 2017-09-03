using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Accounts
{
    public class NetworkAccount : AccountBase
    {
        private readonly AccountBase _account = null;
        private readonly int error_rate = 5;    // error rate: 5%
        private static readonly Random _rnd = new Random();

        public NetworkAccount(AccountBase remoteAccount)
        {
            this._account = remoteAccount;
        }


        public override long GetBalance()
        {
            return this._account.GetBalance();
        }

        public override long Transfer(long value)
        {
            // 有 {error_rate}% 的機率無法執行轉帳, 丟出 NetworkException
            if (_rnd.Next(100) < error_rate / 2) throw new NetworkTransferException();

            this._account.Transfer(value);

            // 有 {error_rate}% 的機率轉帳後無法成功傳回結果, 丟出 NetworkException
            if (_rnd.Next(100) < error_rate / 2) throw new NetworkTransferException();

            return this._account.GetBalance();
        }
    }
}
