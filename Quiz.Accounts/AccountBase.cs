using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Accounts
{
    public abstract class AccountBase
    {
        /// <summary>
        /// 取得帳戶餘額
        /// </summary>
        /// <returns></returns>
        public abstract long GetBalance();


        /// <summary>
        /// 將指定金額轉入該帳戶
        /// </summary>
        /// <param name="transferAmount"></param>
        /// <returns></returns>
        public abstract long Transfer(long transferAmount);


        /// <summary>
        /// 執行多個帳戶之間的轉帳交易。若任一個帳戶執行失敗，所有其他的轉帳動作必須取消。
        /// </summary>
        /// <param name="transes"></param>
        /// <returns></returns>
        public static bool ExecTransaction(params TransactionCmd[] transes)
        {
            {
                // phase 0, check transactions integration
                long total = 0;
                foreach (TransactionCmd tc in transes) total += tc.amount;
                if (total != 0) return false;
            }

            throw new NotImplementedException();
        }
    }
}
