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
            long init1 = 1000;
            long init2 = 1000;

            NetworkAccount acc1 = new NetworkAccount(new MainAccount(1000));
            NetworkAccount acc2 = new NetworkAccount(new MainAccount(1000));
            Random _rnd = new Random();

            for (int i = 0; i < 100000; i++)
            {
                int transfer = _rnd.Next(10) - 5;

                AccountBase.ExecTransaction(
                    new TransactionCmd() { account = acc1, amount = transfer },
                    new TransactionCmd() { account = acc2, amount = 0 - transfer });

            }

            Assert.AreEqual<long>(
                init1+init2,
                acc1.GetBalance() + acc2.GetBalance());


            Console.WriteLine($"Expected: {init1 + init2}");

            while (true) try { init1 = acc1.GetBalance(); break; } catch { }
            while (true) try { init2 = acc2.GetBalance(); break; } catch { }
            Console.WriteLine($"Actual:   {init1 + init2}");
            Console.WriteLine();
        }
    }
}
