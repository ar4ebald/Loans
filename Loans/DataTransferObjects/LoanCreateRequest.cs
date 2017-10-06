using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loans.DataTransferObjects
{
    public class LoanCreateRequest
    {
        public int CreditorId { get; set; }

        public string Description { get; set; }

        public long Amount { get; set; }
    }
}
