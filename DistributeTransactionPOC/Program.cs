//using DistributeTransactionPOC.Accounts;
using Quiz.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributeTransactionPOC
{
    public class Program
    {
        static void Main(string[] args)
        {
            (new Program()).DistributedTransaction();
            (new Program()).ConcurrentTransaction();
        }

        public void DistributedTransaction()
        {
            long init1 = 1000;
            long init2 = 1000;

            NetworkAccount acc1 = new NetworkAccount(new MainAccount(1000));
            NetworkAccount acc2 = new NetworkAccount(new MainAccount(1000));
            Random _rnd = new Random();

            for (int i = 0; i < 100000; i++)
            {
                int transfer = _rnd.Next(10) - 5;

                //MainAccount.Transfer(acc1, acc2, transfer);
                AccountBase.ExecTransaction(
                    new TransactionCmd() { account = acc1, amount = transfer },
                    new TransactionCmd() { account = acc2, amount = 0 - transfer });
            }

            Console.WriteLine($"Expected: {init1 + init2}");
            Console.WriteLine($"Actual: {acc1.GetBalance() + acc2.GetBalance()}");
        }


        public void ConcurrentTransaction()
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

            Console.WriteLine($"Expected Value: {concurrent_threads * repeat_count}");
            Console.WriteLine($"Actual Value:   {q.GetBalance()}");

        }
    }
}
