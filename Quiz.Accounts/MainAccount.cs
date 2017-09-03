using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Accounts
{


    public class MainAccount : AccountBase
    {
        private long _balance = 0;

        public MainAccount(long initBalance)
        {
            this._balance = initBalance;
        }


        public override long GetBalance()
        {
            return this._balance;
        }

        /// <summary>
        /// 呼叫 .Transfer(value), 代表在該戶頭內轉入 value 的金額。
        /// 轉帳完成後餘額 (balance) 會增加 value。
        /// </summary>
        /// <param name="value">轉入金額</param>
        /// <returns>轉帳完成後的餘額</returns>
        public override long Transfer(long value)
        {
            //
            // QUIZ: 確保並行的交易是正確的
            //

            //
            // NOTE: concurrent transaction 的關鍵是 lock, 這個範例只要求本機, 因此只需要用到本機範圍的鎖定機制即可。
            // 如果你無意間發現這個 commit, 歡迎參考我的實作版本。不過在你要抄這份 code 前請先想清楚，在面試時你必須要能
            // 清楚解釋你寫的 code 才行!
            //
            // Andrew Wu
            //
            lock (this)
            {
                if (this._balance + value < 0) throw new BalanceNotEnoughException();
                return (this._balance += value);
            }
        }
    }
}
