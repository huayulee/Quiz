using DistributeTransactionPOC.Accounts;
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

            NetworkMainAccount acc1 = new NetworkMainAccount(1000);
            NetworkMainAccount acc2 = new NetworkMainAccount(1000);
            Random _rnd = new Random();

            for (int i = 0; i < 100000; i++)
            {
                int transfer = _rnd.Next(10) - 5;

                //
                // QUIZ: 確保不可靠的環境下，交易能正確地進行
                //
                try
                {
                    acc1.Transfer(transfer);
                    acc2.Transfer(0 - transfer);
                }
                catch { }
            }


            Console.WriteLine($"Expected: {init1 + init2}");


            while (true) try { init1 = acc1.GetBalance(); break; } catch { }
            while (true) try { init2 = acc2.GetBalance(); break; } catch { }
            Console.WriteLine($"Actual: {init1 + init2}");
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

            Console.WriteLine($"Expected Value: {concurrent_threads * repeat_count}, Actual Value: {q.GetBalance()}");

        }






        
    }






}
