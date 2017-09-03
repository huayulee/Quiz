using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;
using Quiz.Accounts;

namespace Quiz.Accounts.Test
{
    [TestClass]
    public class MainAccountUnitTest
    {
        /// <summary>
        /// NOTE: 同時建立 10000 個 threads, 重複對同一個 account 進行 10000 次的轉帳運算，每次轉入1元到帳號內。
        /// 正確的情況下，原本餘額是0元的帳號，經過多次轉帳後，帳戶餘額應該有 1000000000 元才對。
        /// 
        /// 若未正確處理好 concurrent processing 的問題，測試結果會無法通過。這問題請在多核心的系統上面測試。
        /// </summary>
        [TestMethod]
        public void ConcurrentTransactionTest()
        {
            long concurrent_threads = 10000;
            long repeat_count = 10000;

            List<Thread> threads = new List<Thread>();
            MainAccount q = new MainAccount(0);

            for (int i = 0; i < concurrent_threads; i++)
            {
                Thread t = new Thread(() => { for (int j = 0; j < repeat_count; j++) q.Transfer(1); });
                threads.Add(t);
                t.Start();
            }

            foreach (Thread t in threads) t.Join();

            Assert.AreEqual<long>(
                concurrent_threads * repeat_count,
                q.GetBalance());
        }

        /// <summary>
        /// NOTE: 透過網路對兩個 accounts 進行交易時，必須注意交易的完整性。你必須確保從第一個帳號扣完錢之後，第二個
        /// 帳號務必確認有收到錢，才算完成交易。
        /// 
        /// 若交易失敗，兩個帳號都必須回復原狀。不能發生交易只執行一半的情況發生，如此才能通過單元測試。
        /// </summary>
        [TestMethod]
        public void DistributedTransactionTest()
        {
            int account_count = 5;
            long init_balance = 1000;
            List<AccountBase> accounts = new List<AccountBase>();

            // init account(s)
            for (int i = 0; i < account_count; i++) accounts.Add(new NetworkAccount(new MainAccount(init_balance)));

            Random _rnd = new Random();

            // execute transactions
            for (int i = 0; i < 100000; i++)
            {
                // prepare transaction cmds
                List<TransactionCmd> cmds = new List<TransactionCmd>();
                long total_amount = 0;
                for (int j = 0; j < (account_count - 1); j++)
                {
                    TransactionCmd current_tc = new TransactionCmd()
                    {
                        account = accounts[j],
                        amount = _rnd.Next(10) - 5
                    };
                    total_amount += current_tc.amount;
                    cmds.Add(current_tc);
                }
                cmds.Add(new TransactionCmd()
                {
                    account = accounts[accounts.Count - 1],
                    amount = 0 - total_amount
                });

                // execute transaction
                AccountBase.ExecTransaction(cmds.ToArray());
            }

            // check result
            long total_balances = 0;
            foreach (AccountBase a in accounts) total_balances += a.GetBalance();

            Assert.AreEqual<long>(
                account_count * init_balance,
                total_balances);
        }
    }
}
