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

        /// <summary>
        /// 交易過程發生例外狀況的機率 (%)
        /// </summary>
        private const int error_rate = 5;


        public override long GetBalance()
        {
            // 有 {error_rate}% 的機率無法成功傳回 balance, 丟出 NetworkException
            if (_rnd.Next(100) < error_rate) throw new NetworkException();
            return base.GetBalance();
        }

        public override long Transfer(long value)
        {
            // 有 {error_rate}% 的機率無法執行轉帳, 丟出 NetworkException
            if (_rnd.Next(100) < error_rate / 2) throw new NetworkException();

            base.Transfer(value);

            // 有 {error_rate}% 的機率轉帳後無法成功傳回結果, 丟出 NetworkException
            if (_rnd.Next(100) < error_rate / 2) throw new NetworkException();

            return this._balance;
        }
    }
}
