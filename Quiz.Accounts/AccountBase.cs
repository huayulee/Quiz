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

				if (transes.Length <= 1) return false;

				var twoRandomNumber = GenTwoRandomNumber(transes.Length);		// 隨機取兩個帳號
				var fromAccount = transes[twoRandomNumber.Item1].account;		// 轉出帳號
				var receiveAccount = transes[twoRandomNumber.Item2].account;	// 轉入帳號
				var transAmount = transes[twoRandomNumber.Item1].amount;		// 交易金額

				Transfer2Accounts(fromAccount, receiveAccount, transAmount);
			}

			return true;
		}

		private static Tuple<int, int> GenTwoRandomNumber(int maxNumber)
		{
			Random _rnd = new Random();
			var number1 = _rnd.Next(maxNumber);
			var number2 = _rnd.Next(maxNumber);
			while (number1 == number2) number2 = _rnd.Next(maxNumber);
			return new Tuple<int, int>(number1, number2);
		}

		private static void Transfer2Accounts(AccountBase from, AccountBase receive, long amount)
		{
			DoTransferUntilSuccess(from, amount);
			DoTransferUntilSuccess(receive, 0 - amount);
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
