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



            //
            // NOTE: 協調多個帳號之間的交易機制，主要是要靠兩階段提交 (two phase commit) 的機制來解決。第一階段要協調每個交易的參與者，
            // 必須在一定的時間內明確回覆能否同意這項交易? 協調者在一定時間內確認每個參與者都同意的話，第二階段就要開始執行交易。
            // 到了第二階段，就必須確認交易已經成功為止。若沒有接到回應或確認，則必須不斷重試。
            //
            // 如果你無意間發現這個 commit, 歡迎參考我的實作版本。不過在你要抄這份 code 前請先想清楚，在面試時你必須要能
            // 清楚解釋你寫的 code 才行!
            //
            // Andrew Wu
            //
            {
                // phase 1, make sure transaction execution
                foreach (TransactionCmd tc in transes)
                {
                    if ((tc.account.GetBalance() + tc.amount) < 0) return false;
                }
            }

            {
                // phase 2, execute transactions until success
                foreach (TransactionCmd tc in transes)
                {
                    DoTransferUntilSuccess(tc.account, tc.amount);
                }
                return true;
            }
        }

        private static void DoTransferUntilSuccess(AccountBase from, long amount)
        {
            while (true)
            {
                long expectedAmount = from.GetBalance() + amount;
                try
                {
                    from.Transfer(amount);
                    break;
                }
                catch (NetworkTransferException)
                {
                    if (from.GetBalance() == expectedAmount) break;
                }
            }
            return;
        }
    }
}
