using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Accounts
{
    public class TransactionCmd
    {
        public AccountBase account { get; set; }
        public long amount { get; set; }
    }
}
