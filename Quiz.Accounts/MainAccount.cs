using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quiz.Accounts
{

    public class MainAccount : AccountBase
    {
		private static object _objLock = new object();
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
			lock (_objLock)
			{
				this._balance += value;
			}

			return this.GetBalance();
			//throw new NotImplementedException();
		}
	}
}
