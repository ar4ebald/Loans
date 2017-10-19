using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loans.DataTransferObjects.Loan
{
    public class LoanByIdResponse
    {
        public int Id { get; set; }

        public int DebitorId { get; set; }

        public int CreditorId { get; set; }

        public long Amount { get; set; }

        public DateTimeOffset Time { get; set; }

        public string Description { get; set; }
    }
}
